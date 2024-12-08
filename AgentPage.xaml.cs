using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace БатталовГлазкиSave
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        private bool isLoaded = false;

        public AgentPage()
        {
            InitializeComponent();
            isLoaded = true;

            var currentAgents = БатталовГлазкиSaveEntities.GetContext().Agent.ToList();
            AgentListView.ItemsSource = currentAgents;
            ComboFilter.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        public void Update()
        {
            var agents = БатталовГлазкиSaveEntities.GetContext().Agent.ToList();

            switch (ComboFilter.SelectedIndex)
            {
                case 1:
                    agents = agents.OrderBy(a => a.Title).ToList();
                    break;
                case 2:
                    agents = agents.OrderByDescending(a => a.Title).ToList();
                    break;
                case 5:
                    agents = agents.OrderBy(a => a.Priority).ToList();
                    break;
                case 6:
                    agents = agents.OrderByDescending(a => a.Priority).ToList();
                    break;
            }
            switch (ComboType.SelectedIndex)
            {
                case 1:
                    agents = agents.Where(a => a.AgentTypeTitle == "МФО").ToList();
                    break;
                case 2:
                    agents = agents.Where(a => a.AgentTypeTitle == "ООО").ToList();
                    break;
                case 3:
                    agents = agents.Where(a => a.AgentTypeTitle == "ЗАО").ToList();
                    break;
                case 4:
                    agents = agents.Where(a => a.AgentTypeTitle == "МФО").ToList();
                    break;
                case 5:
                    agents = agents.Where(a => a.AgentTypeTitle == "МКК").ToList();
                    break;
                case 6:
                    agents = agents.Where(a => a.AgentTypeTitle == "ОАО").ToList();
                    break;
            }

            agents = agents.Where(a=>a.Title.ToLower().Contains(SearchTextBox.Text.ToLower()) || a.Email.ToLower().Contains(SearchTextBox.Text.ToLower()) || a.Phone.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ","").Replace("+7", "8").Contains(SearchTextBox.Text.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("+7","8"))).ToList();

            AgentListView.ItemsSource = agents;
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded)
                return;
            Update();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded)
                return;
            Update();
        }

        private void SearchTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
                return;
            Update();
        }
    }
}
