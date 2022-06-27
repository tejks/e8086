﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        static Regex Validator = new("^[0-9A-F]");


        public MainWindow()
        {
            InitializeComponent();

            var registerNames = new[] { "AH", "AL", "BH", "BL", "CH", "CL", "DH", "DL" };
            foreach (var reg in registerNames)
                Registers.Add(new Register { Name = reg, Value = "00" });

            OneRegisterOperationList.ItemsSource = Registers;
        }

        public void Clear(object sender, RoutedEventArgs e)
        {
            foreach (Register register in Registers)
            {
                if (FindName(register.Name) is not TextBox input) continue;
                input.Text = "";
                ClearInput(input);
            }
        }

        public void Random(object sender, RoutedEventArgs e)
        {
            foreach (Register register in Registers)
            {
                if (FindName(register.Name) is not TextBox input) continue;
                input.Text = RandomHexGenerator8Bit();
                ClearInput(input);
            }
        }

        public void Move(object sender, RoutedEventArgs e)
        {
            foreach (Register register in Registers)
            {
                if (FindName(register.Name) is not TextBox input) continue;
                if (FindName(register.Name + "r") is not TextBlock moveTo) continue;

                moveTo.Text = "00";

                var text = input.Text;

                if (HexValidator(text) && !string.IsNullOrWhiteSpace(text))
                {
                    InputOk(input);

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

        public void IncOperation(object sender, RoutedEventArgs e)
        {
            if ((Register)OneRegisterOperationList.SelectedItem is Register register)
            {
                if (FindName(register.Name + "r") is not TextBlock input) return;

                int data = int.Parse(register.Value, NumberStyles.HexNumber) + 1;

                if (data != 256)
                    register.Value = data.ToString("X");
                else
                    register.Value = "00";

                input.Text = register.Value;
            }
        }

        public class Register
        {
            public string Name { get; init; }
            public string Value { get; set; }
        }

                private void InputOk(TextBox input)
        {
            input.BorderThickness = new Thickness(2);
            input.BorderBrush = Brushes.Green;
        }

        private void InputError(TextBox input)
        {
            input.BorderThickness = new Thickness(2);
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
