
using System;
using System.Windows;
using System.Windows.Controls;
using Lexipun.Templates.FilledUIELements.FilledUIELements;

namespace Lexipun.Templates.Pages
{
    /// <summary>
    /// Interaction logic for MainFactoryForAddOREdit.xaml
    /// </summary>
    public partial class PopUpWindowForFillOrChangeDataTemplate : Window//, ISenderFromCheck, ICheckOnCreatedThisCombinationOfUniquesesField
    {
        private TemplateOfRenderFields templateOfRenderFields;

        public string MessageChoseFile { get=>templateOfRenderFields.MessageChoseFile; set=> templateOfRenderFields.MessageChoseFile = value; }
        public string MessageIncorrectData { get=> templateOfRenderFields.MessageIncorrectData; set=> templateOfRenderFields.MessageIncorrectData = value; }
        public string FileFilter { get => templateOfRenderFields.FileFilter; set => templateOfRenderFields.FileFilter = value; }
        public bool AllValuesIsUnique { get => templateOfRenderFields.AllValuesIsUnique;  }
        public Func<string, string, bool> CheckOnExistValue { get=> templateOfRenderFields.CheckOnExistValue; set => templateOfRenderFields.CheckOnExistValue = value; }
        public Action AditionalActOnSave { get; set; }
        public string ButtonCancel { get; set; }
        public PopUpWindowForFillOrChangeDataTemplate(object sourceObject)
        {
            InitializeComponent();

            templateOfRenderFields = new TemplateOfRenderFields(sourceObject);

            WrapPanel RenderedContentOfWindow = templateOfRenderFields.RenderOfFields();

            Grid.SetColumnSpan(RenderedContentOfWindow, 4);

            gridOfWindow.Children.Add(RenderedContentOfWindow);

            ButtonCancel = "Cancel";
        }

        public void Update()
        {
            gridOfWindow.Children.RemoveAt(gridOfWindow.Children.Count - 1);

            WrapPanel RenderedContentOfWindow = templateOfRenderFields.RenderOfFields();

            Grid.SetColumnSpan(RenderedContentOfWindow, 4);

            gridOfWindow.Children.Add(RenderedContentOfWindow);

            Cancel.Content = ButtonCancel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonOfAct_Click(object sender, RoutedEventArgs e)
        {
            if (AditionalActOnSave != null)
            {
                AditionalActOnSave();
                return;
            }

            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


    }
}
