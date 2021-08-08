
using Lexipun.DotNetFramework.DataProcessing;
using Lexipun.Templates.Atributes;
using Lexipun.Templates.Interfaces;
using Lexipun.Templates.Pages;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lexipun.Templates.FilledUIELements.FilledUIELements
{
    public class TemplateOfRenderFields
    {
        private readonly PropertyProcessing<object> propertiesOfObject;
        WrapPanel listView = new WrapPanel();

        private readonly List<TextBlock> textBlocks = new List<TextBlock>();
        private readonly List<UIElement> uIElements = new List<UIElement>();
        private readonly List<int> indexesOfMarkedFieldsForUpdating = new List<int>();
        private readonly List<int> indexToIndexOfProperties = new List<int>();
        private readonly Dictionary<int, int> indexUniqueIdPair = new Dictionary<int, int>();

        public string MessageChoseFile { get; set; }
        public string MessageIncorrectData { get; set; }
        public string FileFilter { get; set; }
        public bool AllValuesIsUnique { get; private set; }
        public bool IsReadOnly { get; }

        /// <summary>
        /// first value is name of Property
        /// second value is value of property
        /// 
        /// return 
        /// true - exist this value
        /// false - not exist this value
        /// </summary>
        public Func<string, string, bool> CheckOnExistValue { get; set; }


        public TemplateOfRenderFields(object sourceObject)
        {
            MessageChoseFile = "Chose file";
            MessageIncorrectData = "IncorrectData";
            FileFilter = "";

            propertiesOfObject = new PropertyProcessing<object>(sourceObject);
            IsReadOnly = false;
        }

        public TemplateOfRenderFields(object sourceObject, in bool isReadonly)
        {
            MessageChoseFile = "Chose file";
            MessageIncorrectData = "IncorrectData";
            FileFilter = "";

            propertiesOfObject = new PropertyProcessing<object>(sourceObject);
            IsReadOnly = isReadonly;
        }


        public WrapPanel RenderOfFields()
        {
            listView.Children.Clear();
            textBlocks.Clear();
            uIElements.Clear();
            indexesOfMarkedFieldsForUpdating.Clear();
            indexToIndexOfProperties.Clear();
            indexUniqueIdPair.Clear();


            propertiesOfObject.Reset();
            Dictionary<string, string> associativeNamesOfFields = null;
            IAssociativeNamesOfProperties associativeNames = propertiesOfObject.SourceObject as IAssociativeNamesOfProperties;

            if (associativeNames != null)
            {
                associativeNamesOfFields = associativeNames.GetListOfFieldsNames();
            }

            WrapPanel wrapperOfField;
            TextBlock nameOfField;
            TextBlock messagingAboutWarning;
            Border wrapperForContentOfField;
            UIElement contentOfField;

           while( propertiesOfObject.MoveNext() )
            {

                if (propertiesOfObject.GetAttributeOfCurrentProperty(typeof(NonGenerateByTemplate)) != null)
                {
                    continue;
                }

                contentOfField = new TextBox();
                wrapperForContentOfField = new Border();
                messagingAboutWarning = new TextBlock();
                nameOfField = new TextBlock();
                wrapperOfField = new WrapPanel();

                messagingAboutWarning.Foreground = Brushes.Red;

                if (associativeNamesOfFields != null && associativeNamesOfFields.ContainsKey(propertiesOfObject.GetCurrentNameOfProperty()))
                {
                    nameOfField.Text = associativeNamesOfFields[propertiesOfObject.GetCurrentNameOfProperty()];
                }
                else
                {
                    nameOfField.Text = propertiesOfObject.GetCurrentNameOfProperty();
                }

                (contentOfField as TextBox).Text = Convert.ToString(propertiesOfObject.GetCurrent());
                (contentOfField as TextBox).LostFocus += ValidateTypedData_LostFocus;
                (contentOfField as TextBox).GotFocus += UnsetWarningForTextBoxAfterAct_GotFocus;
                indexToIndexOfProperties.Add(propertiesOfObject.GetCurrentPosition);

                #region SavePropertiesWithAttributes

                if (propertiesOfObject.GetAttributeOfCurrentProperty(typeof(AutoUpdateByTemplate)) != null)
                {
                    indexesOfMarkedFieldsForUpdating.Add(indexToIndexOfProperties.Count - 1);
                }

                if (propertiesOfObject.GetAttributeOfCurrentProperty(typeof(IsUnique)) != null)
                {
                    indexUniqueIdPair.Add(indexToIndexOfProperties.Count - 1, (propertiesOfObject.GetAttributeOfCurrentProperty(typeof(IsUnique)) as IsUnique).Id);
                }

                #endregion SavePropertiesWithAttributes

                SetAdditionSettingsForUIElement(ref contentOfField);

                wrapperForContentOfField.Child = contentOfField; //wrap in Border for set red thickness

                //wrap name, value, message (for possibility say about incorrect data) in one value;
                wrapperOfField.Children.Add(nameOfField);
                wrapperOfField.Children.Add(wrapperForContentOfField);
                wrapperOfField.Children.Add(messagingAboutWarning);

                // save name and value for more easer access to them
                textBlocks.Add(nameOfField);
                uIElements.Add(contentOfField);


                listView.Children.Add(wrapperOfField); //display it

                bool IsSetToWarningOfAutoupdatingValue = propertiesOfObject.GetAttributeOfCurrentProperty(typeof(AutoUpdateByTemplate)) != null
                    && propertiesOfObject.GetCurrentPropertysLevelOfAccessToSet().IsPublic;
                bool IsSetToWarningOfUniqueValue = propertiesOfObject.GetAttributeOfCurrentProperty(typeof(IsUnique)) != null;

                bool IsEmptyField = string.IsNullOrEmpty(Convert.ToString(propertiesOfObject.GetCurrent()));

            }

            return listView;
        }

        private void UnsetWarningForTextBoxAfterAct_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Border border = textBox.Parent as Border;

            WrapPanel wrapPanel = border.Parent as WrapPanel;

            UnsetWarning(wrapPanel);
        }

        private void ValidateTypedData_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Border border = textBox.Parent as Border;

            if (border is null)
            {
                Grid grid = textBox.Parent as Grid;

                if (grid is null)
                {
                    WrapPanel wrapPanel1 = textBox.Parent as WrapPanel;
                    border = wrapPanel1.Parent as Border;
                }
                else
                {

                    border = grid.Parent as Border;
                }
            }

            WrapPanel wrapPanel = border.Parent as WrapPanel;
            int indexOfField = listView.Children.IndexOf(wrapPanel);
            string savedData;
            string result;

            propertiesOfObject.MoveTo(indexToIndexOfProperties[indexOfField]);

            savedData = Convert.ToString(propertiesOfObject.GetCurrent());

            if (savedData.Equals(textBox.Text))
            {
                return;
            }

            propertiesOfObject.SetCurrent(textBox.Text);

            result = Convert.ToString(propertiesOfObject.GetCurrent());

            if (result.Equals(savedData))
            {
                SetWarning(wrapPanel);
                return;
            }

            UnsetWarning(wrapPanel);
            UpdateMarckedValues(textBox);
            CheckOnUnique(indexOfField);
        }

        /// <summary>
        ///  methos for check on unique values
        /// </summary>
        /// <param name="indexOfField"></param>
        private void CheckOnUnique(in int indexOfField)
        {
            if (!indexUniqueIdPair.ContainsKey(indexOfField))
            {
                return;
            }

            int id = indexUniqueIdPair[indexOfField];

            foreach (int index in indexUniqueIdPair.Where(SelectionById).Select(Selection))
            {
                propertiesOfObject.MoveTo(indexToIndexOfProperties[index]);

                string propertyName = propertiesOfObject.GetCurrentNameOfProperty();
                string valueOFProperty = Convert.ToString(propertiesOfObject.GetCurrent());


                if (!CheckOnExistValue(propertyName, valueOFProperty))
                {
                    AllValuesIsUnique = false;
                    return;
                }

            }

            AllValuesIsUnique = true;

            int Selection(KeyValuePair<int, int> keyValuePair)
            {
                return keyValuePair.Key;
            }

            bool SelectionById(KeyValuePair<int, int> keyValuePair)
            {
                if (keyValuePair.Value == id)
                {
                    return true;
                }
                return false;
            }
        }

        private void UpdateMarckedValues(TextBox textBox)
        {
            string savedData;
            for (int i = 0; i < indexesOfMarkedFieldsForUpdating.Count; ++i)
            {
                propertiesOfObject.MoveTo(indexToIndexOfProperties[indexesOfMarkedFieldsForUpdating[i]]);

                WrapPanel wrapperOfField = listView.Children[indexesOfMarkedFieldsForUpdating[i]] as WrapPanel;
                Border wrapperForContentOfField = wrapperOfField.Children[1] as Border;

                if (!(wrapperForContentOfField.Child is TextBox contentOfField))
                {
                    continue;
                }

                savedData = Convert.ToString(propertiesOfObject.GetCurrent());

                if (contentOfField == textBox)
                {
                    contentOfField.Background = Brushes.Transparent;
                    continue;
                }

                if (savedData.Equals(contentOfField.Text))
                {
                    continue;
                }

                contentOfField.Text = savedData;

                contentOfField.Background = Brushes.LightGray;
                UnsetWarning(wrapperOfField);
            }
        }

        private void UpdateMarckedValues()
        {
            string savedData;
            for (int i = 0; i < indexesOfMarkedFieldsForUpdating.Count; ++i)
            {
                propertiesOfObject.MoveTo(indexToIndexOfProperties[indexesOfMarkedFieldsForUpdating[i]]);

                WrapPanel wrapperOfField = listView.Children[indexesOfMarkedFieldsForUpdating[i]] as WrapPanel;
                Border wrapperForContentOfField = wrapperOfField.Children[1] as Border;

                if (!(wrapperForContentOfField.Child is TextBox contentOfField))
                {
                    continue;
                }

                savedData = Convert.ToString(propertiesOfObject.GetCurrent());

                if (savedData.Equals(contentOfField.Text))
                {
                    continue;
                }

                contentOfField.Text = savedData;

                contentOfField.Background = Brushes.LightGray;

            }
        }

        private void SetWarning(WrapPanel wrapPanel)
        {
            Border border = wrapPanel.Children[1] as Border;
            TextBlock textBlock = wrapPanel.Children[2] as TextBlock;

            border.BorderBrush = Brushes.Red;
            textBlock.Text = MessageIncorrectData;
        }

        private void UnsetWarning(WrapPanel wrapPanel)
        {
            Border border = wrapPanel.Children[1] as Border;
            TextBlock textBlock = wrapPanel.Children[2] as TextBlock;

            border.BorderBrush = Brushes.Transparent;
            textBlock.Background = Brushes.Transparent;
            textBlock.Text = "";
        }

        private void SetAdditionSettingsForUIElement(ref UIElement contentOfField)
        {
            TextBox textBox = contentOfField as TextBox;


            if (propertiesOfObject.GetAttributeOfCurrentProperty(typeof(GenerateAs)) is GenerateAs generateAs)
            {
                switch (generateAs.Generated)
                {
                    case TypeOfGeneration.Path:
                        {
                            WrapPanel wrapPanel = new WrapPanel();
                            Button button = new Button();
                            TextBox textBox1 = new TextBox();
                            Thickness thickness = new Thickness(2);

                            button.Content = MessageChoseFile;
                            button.HorizontalAlignment = HorizontalAlignment.Left;
                            button.Margin = thickness;
                            button.Click += ChoseFile_Click;
                            textBox1.Background = Brushes.LightGray;
                            textBox1.Focusable = false;
                            textBox1.Text = textBox.Text;
                            textBox1.Margin = thickness;

                            wrapPanel.Children.Add(button);
                            wrapPanel.Children.Add(textBox1);

                            contentOfField = wrapPanel;

                            if (IsReadOnly)
                            {
                                button.IsEnabled = false;
                                textBox1.Background = Brushes.LightGray;
                                textBox1.Focusable = false;
                            }

                            void ChoseFile_Click(object sender, RoutedEventArgs e)
                            {
                                Button button1 = sender as Button;
                                WrapPanel wrapPanel1 = button1.Parent as WrapPanel;
                                TextBox textBox2 = wrapPanel1.Children[1] as TextBox;
                                Border border = wrapPanel1.Parent as Border;
                                WrapPanel wrapPanel2 = border.Parent as WrapPanel;
                                int currentIndex = listView.Children.IndexOf(wrapPanel2);

                                propertiesOfObject.MoveTo(indexToIndexOfProperties[currentIndex]);

                                OpenFileDialog openfileDialog = new OpenFileDialog()
                                {
                                    Filter = generateAs.WhichFiles,
                                };

                                bool? result = openfileDialog.ShowDialog();

                                if (result.Value)
                                {
                                    propertiesOfObject.SetCurrent(openfileDialog.FileName);

                                    textBox2.Text = Convert.ToString(propertiesOfObject.GetCurrent());
                                }
                            }
                        }
                        break;
                    case TypeOfGeneration.Password:
                        {
                            contentOfField = new PasswordBox();
                            (contentOfField as PasswordBox).Password = textBox.Text;
                            contentOfField.LostFocus += Password_LostFocus;

                            if (IsReadOnly)
                            {

                                (contentOfField as PasswordBox).Background = Brushes.LightGray;
                                contentOfField.Focusable = false;
                            }
                        }
                        break;
                    case TypeOfGeneration.NumericUpDown:
                        {
                            Grid grid = new Grid();
                            Thickness thickness = new Thickness(0);

                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            grid.ColumnDefinitions.Add(columnDefinition);
                            columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(14);
                            grid.ColumnDefinitions.Add(columnDefinition);

                            RowDefinition rowDefinition = new RowDefinition();
                            grid.RowDefinitions.Add(rowDefinition);
                            rowDefinition = new RowDefinition();
                            grid.RowDefinitions.Add(rowDefinition);

                            TextBox textBox1 = new TextBox();
                            Button button = new Button();
                            Button button1 = new Button();

                            Grid.SetRowSpan(textBox1, 2);
                            Grid.SetColumn(button, 1);
                            Grid.SetColumn(button1, 1);
                            Grid.SetRow(button1, 1);

                            textBox1.TextAlignment = TextAlignment.Right;
                            button.FontSize = button.FontSize / 3;
                            button1.FontSize = button1.FontSize / 3;
                            textBox1.Text = textBox.Text;
                            button.Content = "▲";
                            button1.Content = "▼";
                            button.Click += NumericUpDow_Up_Click;
                            button1.Click += NumericUpDown_Down_Click;
                            textBox1.LostFocus += ValidateTypedData_LostFocus;
                            button.Margin = thickness;
                            button1.Margin = thickness;

                            grid.Children.Add(textBox1);
                            grid.Children.Add(button);
                            grid.Children.Add(button1);

                            contentOfField = grid;

                            if (IsReadOnly)
                            {
                                button.IsEnabled = false;
                                button1.IsEnabled = false;

                                textBox1.Background = Brushes.LightGray;
                                textBox1.Focusable = false;
                            }
                        }
                        break;
                }

            }
            else if (propertiesOfObject.GetCurrentType().IsEnum)
            {
                contentOfField = new ComboBox();
                Array array = Enum.GetValues(propertiesOfObject.GetCurrentType());
                IEnumerator enumerator = array.GetEnumerator();

                enumerator.MoveNext();

                (contentOfField as ComboBox).ItemsSource = array;
                (contentOfField as ComboBox).SelectedItem = enumerator.Current;
                (contentOfField as ComboBox).SelectionChanged += SetValue_SelectionChanged; 
                (contentOfField as ComboBox).IsReadOnly = IsReadOnly;
            }
            else if (propertiesOfObject.GetCurrent() as IComboBox != null)
            {
                contentOfField = new ComboBox();

                (contentOfField as ComboBox).ItemsSource = (propertiesOfObject.GetCurrent() as IComboBox).GetArray();
                (contentOfField as ComboBox).SelectedItem = (propertiesOfObject.GetCurrent() as IComboBox).ChosenItem;
                (contentOfField as ComboBox).SelectionChanged += SetValue_SelectionChanged;
                (contentOfField as ComboBox).IsReadOnly = IsReadOnly;
                
            }

            if (IsReadOnly || !propertiesOfObject.GetCurrentPropertysLevelOfAccessToSet().IsPublic)
            {
                textBox.Background = Brushes.LightGray;
                textBox.Focusable = false;
            }
        }

        private void NumericUpDown_Down_Click(object sender, RoutedEventArgs e)
        {
            Button repeatButton = sender as Button;
            Grid parentGrid = repeatButton.Parent as Grid;
            TextBox textOfNumber = parentGrid.Children[0] as TextBox;

            if (string.IsNullOrEmpty(textOfNumber.Text))
            {
                textOfNumber.Text = "0";
            }

            double countHours = double.Parse(textOfNumber.Text);

            textOfNumber.Text = (countHours - 1).ToString();
            ValidateTypedData_LostFocus(textOfNumber, null);
        }

        private void NumericUpDow_Up_Click(object sender, RoutedEventArgs e)
        {
            Button repeatButton = sender as Button;
            Grid parentGrid = repeatButton.Parent as Grid;
            TextBox textOfNumber = parentGrid.Children[0] as TextBox;

            if (string.IsNullOrEmpty(textOfNumber.Text))
            {
                textOfNumber.Text = "0";
            }

            double number = double.Parse(textOfNumber.Text);

            textOfNumber.Text = (number + 1).ToString();
            ValidateTypedData_LostFocus(textOfNumber, null);
        }

        /// <summary>
        /// it need to optimize because code copied from method ValidateTypedData_LostFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox textBox = sender as PasswordBox;
            Border border = textBox.Parent as Border;
            WrapPanel wrapPanel = border.Parent as WrapPanel;
            int indexOfField = listView.Children.IndexOf(wrapPanel);
            string savedData;
            string result;

            propertiesOfObject.MoveTo(indexToIndexOfProperties[indexOfField]);

            savedData = Convert.ToString(propertiesOfObject.GetCurrent());

            if (savedData.Equals(textBox.Password))
            {
                return;
            }

            propertiesOfObject.SetCurrent(textBox.Password);

            result = Convert.ToString(propertiesOfObject.GetCurrent());

            if (result.Equals(savedData))
            {
                SetWarning(wrapPanel);
                return;
            }

            UnsetWarning(wrapPanel);
        }

        private void SetValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            Border border = comboBox.Parent as Border;
            WrapPanel wrapPanel = border.Parent as WrapPanel;

            int currentIndex = listView.Children.IndexOf(wrapPanel);

            propertiesOfObject.MoveTo(indexToIndexOfProperties[currentIndex]);

            if (propertiesOfObject.GetCurrent() as IComboBox != null)
            {
                (propertiesOfObject.GetCurrent() as IComboBox).ChosenItem = comboBox.SelectedItem;
                propertiesOfObject.SetCurrent(propertiesOfObject.GetCurrent());

                UpdateMarckedValues();
                return;
            }

            propertiesOfObject.SetCurrent(comboBox.SelectedItem);

            UpdateMarckedValues();
        }

        private void TemplateOfRenderFields_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateTypedData_LostFocus(sender, e);
        }
    }
}
