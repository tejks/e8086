using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace e8086
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Register> Registers = new();

        public MainWindow()
        {
            InitializeComponent();

            var registerNames = new[] { "AH", "AL", "BH", "BL", "CH", "CL", "DH", "DL" };
            foreach (var reg in registerNames)
                Registers.Add(new Register { Name = reg, Value = "00" });

            OneRegisterOperationList.ItemsSource = Registers;
        }

        public void ClearOperation(object sender, RoutedEventArgs e)
        {
            foreach (var reg in Registers)
            {
                var input = FindName(reg.Name) as TextBox;
                if (input == null) continue;
                input.Text = "";
                input.ClearValue(Border.BorderBrushProperty);
                input.ClearValue(Border.BorderThicknessProperty);
            }
        }

        public void RandomOperation(object sender, RoutedEventArgs e)
        {
            foreach (var reg in Registers)
            {
                var input = FindName(reg.Name) as TextBox;
                if (input == null) continue;
                input.ClearValue(Border.BorderBrushProperty);
                input.ClearValue(Border.BorderThicknessProperty);
                input.Text = RandomHexGenerator8Bit();
            }
        }

        public void MovAllOperation(object sender, RoutedEventArgs e)
        {
            foreach (var reg in Registers)
            {
                var input = FindName(reg.Name);
                var inputChild = input as TextBox;

                var output = FindName(reg.Name + "r");
                var outputChild = output as TextBlock;

                if (inputChild != null && !string.IsNullOrWhiteSpace(inputChild.Text))
                {
                    if (HexValidator(inputChild.Text.ToUpper()))
                    {
                        inputChild.BorderBrush = Brushes.Green;
                        inputChild.BorderThickness = new Thickness(2);

                        if (inputChild.Text.Length == 1) inputChild.Text = "0" + inputChild.Text.ToUpper();
                        reg.Value = inputChild.Text.ToUpper();
                        if (outputChild != null) outputChild.Text = inputChild.Text.ToUpper();
                    }
                    else
                    {
                        inputChild.BorderBrush = Brushes.Red;
                        inputChild.BorderThickness = new Thickness(1.5);
                    }
                }
                else
                {
                    if (outputChild != null) outputChild.Text = "00";
                }
            }
        }

        public static string RandomHexGenerator8Bit()
        {
            const string chars = "0123456789ABCDEF";
            var rand = new Random();
            return new string(Enumerable.Repeat(chars, 2).Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        public static bool HexValidator(string input)
        {
            for (var i = 0; i < input.Length; i++)
                if (!(input[i] >= '0' && input[i] <= '9' || input[i] >= 'A' && input[i] <= 'F'))
                    return false;
            return true;
        }

        public void IncOperation(object sender, RoutedEventArgs e)
        {
            var singleReg = (Register)OneRegisterOperationList.SelectedItem;

            if (singleReg != null)
            {
                var input = FindName(singleReg.Name + "r");
                var inputChild = input as TextBlock;
                var intFromHex = int.Parse(singleReg.Value, NumberStyles.HexNumber) + 1;

                if (intFromHex == 256)
                    singleReg.Value = "00";
                else
                    singleReg.Value = intFromHex.ToString("X").PadLeft(2, '0');

                inputChild.Text = singleReg.Value.ToUpper().PadLeft(2, '0');
            }
            else
            {
                MessageBox.Show("Wybierz rejestr!");
            }
        }

        public void DecOperation(object sender, RoutedEventArgs e)
        {
            var singleReg = (Register)OneRegisterOperationList.SelectedItem;

            if (singleReg != null)
            {
                var input = FindName(singleReg.Name + "r");
                var inputChild = input as TextBlock;
                var intFromHex = int.Parse(singleReg.Value, NumberStyles.HexNumber) - 1;

                if (intFromHex == -1)
                    singleReg.Value = "FF";
                else
                    singleReg.Value = intFromHex.ToString("X").PadLeft(2, '0');

                inputChild.Text = singleReg.Value.ToUpper().PadLeft(2, '0');
            }
            else
            {
                MessageBox.Show("Wybierz rejestr!");
            }
        }

        public void NotOperation(object sender, RoutedEventArgs e)
        {
            var singleReg = (Register)OneRegisterOperationList.SelectedItem;

            if (singleReg != null)
            {
                var input = FindName(singleReg.Name + "r");
                var inputChild = input as TextBlock;
                var binaryString = string.Join(string.Empty,
                    singleReg.Value.Select(c =>
                        Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                var binaryNot = string.Concat(binaryString.Select(x => x == '0' ? '1' : '0'));

                singleReg.Value = Convert.ToInt32(binaryNot, 2).ToString("X").PadLeft(2, '0');
                inputChild.Text = singleReg.Value.ToUpper().PadLeft(2, '0');
            }
            else
            {
                MessageBox.Show("Wybierz rejestr!");
            }
        }

        public void NegOperation(object sender, RoutedEventArgs e)
        {
            var singleReg = (Register)OneRegisterOperationList.SelectedItem;

            if (singleReg != null)
            {
                var input = FindName(singleReg.Name + "r");
                var inputChild = input as TextBlock;

                NotOperation(sender, e);
                IncOperation(sender, e);

                inputChild.Text = singleReg.Value.ToUpper().PadLeft(2, '0');
            }
            else
            {
                MessageBox.Show("Wybierz rejestr!");
            }
        }

        public class Register
        {
            public string Name { get; init; }
            public string Value { get; set; }
        }
    }
}
