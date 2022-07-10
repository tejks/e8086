using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace e8086
{
    public partial class MainWindow : Window
    {
        public List<Register> Registers = new();
        static Regex Validator = new("^[0-9A-F]");


        public MainWindow()
        {
            InitializeComponent();

            var registerNames = new[] { "AH", "AL", "BH", "BL", "CH", "CL", "DH", "DL" };
            foreach (var reg in registerNames)
                Registers.Add(new Register { Type = reg, Value = "00" });

            SingleOperations.ItemsSource = Registers;
            DoubleRegistersX.ItemsSource = Registers;
            DoubleRegistersY.ItemsSource = Registers;
        }

        public void Clear(object sender, RoutedEventArgs e)
        {
            foreach (Register register in Registers)
            {
                if (FindName(register.Type) is not TextBox input) continue;
                input.Text = "";
                ClearInput(input);
            }
        }

        public void Random(object sender, RoutedEventArgs e)
        {
            foreach (Register register in Registers)
            {
                if (FindName(register.Type) is not TextBox input) continue;
                input.Text = RandomHexGenerator8Bit();
                ClearInput(input);
            }
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            foreach (Register register in Registers)
            {
                if (FindName(register.Type) is not TextBox input) continue;
                if (FindName(register.Type + "r") is not TextBlock moveTo) continue;

                ClearInput(input);

                moveTo.Text = "00";
                register.Value = "00";

                var text = input.Text;

                if (HexValidator(text) && !string.IsNullOrWhiteSpace(text))
                {
                    String result;

                    if (text.Length == 1)
                        result = "0" + text;
                    else
                        result = input.Text.ToUpper();

                    register.Value = result;
                    moveTo.Text = result;
                    input.Text = result;
                }
                else
                    InputError(input);
            }
        }

        public void Inc(object sender, RoutedEventArgs e)
        {
            if ((Register)SingleOperations.SelectedItem is Register register)
            {
                if (FindName(register.Type + "r") is not TextBlock input) return;

                int data = int.Parse(register.Value, NumberStyles.HexNumber) + 1;

                if (data != 256)
                    register.Value = data.ToString("X2");
                else
                    register.Value = "00";

                input.Text = register.Value;
            }
        }

        public void Dec(object sender, RoutedEventArgs e)
        {
            if ((Register)SingleOperations.SelectedItem is Register register)
            {
                if (FindName(register.Type + "r") is not TextBlock input) return;

                int data = int.Parse(register.Value, NumberStyles.HexNumber) - 1;

                if (data != -1)
                    register.Value = data.ToString("X2");
                else
                    register.Value = "FF";

                input.Text = register.Value;
            }
        }

        public void Not(object sender, RoutedEventArgs e)
        {
            if ((Register)SingleOperations.SelectedItem is Register register)
            {
                if (FindName(register.Type + "r") is not TextBlock input) return;

                uint bites = byte.Parse(register.Value, NumberStyles.HexNumber);

                var result = (~bites).ToString("X").Substring(6, 2);

                register.Value = result;
                input.Text = result;
            }
        }

        public void Neg(object sender, RoutedEventArgs e)
        {
            if ((Register)SingleOperations.SelectedItem is Register register)
            {
                if (FindName(register.Type + "r") is not TextBlock input) return;

                Not(sender, e);
                Inc(sender, e);

                input.Text = register.Value.ToUpper().PadLeft(2, '0');
            }
        }

        public void AND(object sender, RoutedEventArgs e)
        {
            if ((Register)DoubleRegistersX.SelectedItem is Register registerX && (Register)DoubleRegistersY.SelectedItem is Register registerY)
            {
                if (FindName(registerX.Type + "r") is not TextBlock input) return;

                uint bitesX = byte.Parse(registerX.Value, NumberStyles.HexNumber);
                uint bitesY = byte.Parse(registerY.Value, NumberStyles.HexNumber);

                var result = bitesX & bitesY;

                registerX.Value = result.ToString("X2");
                input.Text = result.ToString("X2");
            }
        }

        public void OR(object sender, RoutedEventArgs e)
        {
            if ((Register)DoubleRegistersX.SelectedItem is Register registerX && (Register)DoubleRegistersY.SelectedItem is Register registerY)
            {
                if (FindName(registerX.Type + "r") is not TextBlock input) return;

                uint bitesX = byte.Parse(registerX.Value, NumberStyles.HexNumber);
                uint bitesY = byte.Parse(registerY.Value, NumberStyles.HexNumber);

                var result = bitesX | bitesY;

                registerX.Value = result.ToString("X2");
                input.Text = result.ToString("X2");
            }
        }

        public void XOR(object sender, RoutedEventArgs e)
        {
            if ((Register)DoubleRegistersX.SelectedItem is Register registerX && (Register)DoubleRegistersY.SelectedItem is Register registerY)
            {
                if (FindName(registerX.Type + "r") is not TextBlock input) return;

                uint bitesX = byte.Parse(registerX.Value, NumberStyles.HexNumber);
                uint bitesY = byte.Parse(registerY.Value, NumberStyles.HexNumber);

                var result = bitesX ^ bitesY;

                registerX.Value = result.ToString("X2");
                input.Text = result.ToString("X2");
            }
        }

        public void ADD(object sender, RoutedEventArgs e)
        {
            if ((Register)DoubleRegistersX.SelectedItem is Register registerX && (Register)DoubleRegistersY.SelectedItem is Register registerY)
            {
                if (FindName(registerX.Type + "r") is not TextBlock input) return;

                var x = Convert.ToInt32(registerX.Value, 16);
                var y = Convert.ToInt32(registerY.Value, 16);

                var summary = x + y;

                var result = summary < 255 ? summary : summary % 256;

                registerX.Value = result.ToString("X2");
                input.Text = result.ToString("X2");
            }
        }

        public void SUB(object sender, RoutedEventArgs e)
        {
            if ((Register)DoubleRegistersX.SelectedItem is Register registerX && (Register)DoubleRegistersY.SelectedItem is Register registerY)
            {
                if (FindName(registerX.Type + "r") is not TextBlock input) return;

                var x = Convert.ToInt32(registerX.Value, 16);
                var y = Convert.ToInt32(registerY.Value, 16);

                var summary = x - y;

                var result = summary >= 0 ? summary : summary + 256;

                registerX.Value = result.ToString("X2");
                input.Text = result.ToString("X2");
            }
        }

        public void MOV(object sender, RoutedEventArgs e)
        {
            if ((Register)DoubleRegistersX.SelectedItem is Register registerX && (Register)DoubleRegistersY.SelectedItem is Register registerY)
            {
                if (FindName(registerX.Type + "r") is not TextBlock inputX) return;
                if (FindName(registerY.Type + "r") is not TextBlock inputY) return;

                inputX.Text = inputY.Text;
                registerX.Value = registerY.Value;

                inputY.Text = "00";
                registerY.Value = "00";
            }
        }

        public void XCHG(object sender, RoutedEventArgs e)
        {
            if ((Register)DoubleRegistersX.SelectedItem is Register registerX && (Register)DoubleRegistersY.SelectedItem is Register registerY)
            {
                if (FindName(registerX.Type + "r") is not TextBlock inputX) return;
                if (FindName(registerY.Type + "r") is not TextBlock inputY) return;

                var temp = registerX.Value;
                inputX.Text = registerY.Value;
                registerX.Value = registerY.Value;

                inputY.Text = temp;
                registerY.Value = temp;
            }
        }

        public class Register
        {
            public string Type { get; init; }
            public string Value { get; set; }
        }

        private void InputError(TextBox input)
        {
            input.BorderThickness = new Thickness(0.5);
            input.BorderBrush = Brushes.Red;
        }

        private void ClearInput(TextBox input)
        {
            input.ClearValue(Border.BorderBrushProperty);
            input.ClearValue(Border.BorderThicknessProperty);
        } 

        public static string RandomHexGenerator8Bit()
        {
            const string chars = "0123456789ABCDEF";
            var rand = new Random();
            return new string(Enumerable.Repeat(chars, 2).Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        public static bool HexValidator(string input)
        {
            foreach (var element in input)
                if (!Validator.IsMatch(element.ToString())) return false;
            
            return true;
        }
    }
}
