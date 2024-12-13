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
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;


        public AgentPage()
        {
            InitializeComponent();
            isLoaded = true;

            var currentAgents = БатталовГлазкиSaveEntities.GetContext().Agent.ToList();
            AgentListView.ItemsSource = currentAgents;
            ComboFilter.SelectedIndex = 0;
            Update();
        }


        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;


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

            agents = agents.Where(a => a.Title.ToLower().Contains(SearchTextBox.Text.ToLower()) || a.Email.ToLower().Contains(SearchTextBox.Text.ToLower()) || a.Phone.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("+7", "8").Contains(SearchTextBox.Text.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("+7", "8"))).ToList();

            AgentListView.ItemsSource = agents.ToList();

            TableList = agents.ToList();

            ChangePage(0, 0, GetCountRecords());
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

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
                return;
            ChangePage(1, null, GetCountRecords());
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
                return;
            ChangePage(2, null, GetCountRecords());
        }

        private void PageListBox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isLoaded)
                return;
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1, GetCountRecords());
        }

        private int GetCountRecords()
        {
            return CountRecords;
        }

        private void ChangePage(int direction, int? selectedPage, int countRecords)
        {
            
            CurrentPageList.Clear();
            countRecords = TableList.Count;

            if (countRecords % 10 > 0)
            {
                CountPage = countRecords / 10 + 1;
            }
            else
            {
                CountPage = countRecords / 10;
            }

            bool ifUpdate = true;
            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage < CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < countRecords ? CurrentPage * 10 + 10 : countRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < countRecords ? CurrentPage * 10 + 10 : countRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            ifUpdate = false;
                        }
                        break;
                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < countRecords ? CurrentPage * 10 + 10 : countRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            ifUpdate = false;
                        }
                        break;
                }
            }

            if (ifUpdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                AgentListView.ItemsSource = CurrentPageList;
                AgentListView.Items.Refresh();
            }
        }
    }
}