using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace БатталовГлазкиSave
{

    public partial class PriorityChangeWindow : Window
    {
        private readonly ListView _agentListView; // Ссылка на ListView из вызывающего окна

        public PriorityChangeWindow(ListView agentListView)
        {
            InitializeComponent();
            _agentListView = agentListView; // Сохраняем ссылку на ListView
            var selectedAgents = _agentListView.SelectedItems.Cast<Agent>().ToList();

            // Проверяем, есть ли выбранные элементы
            if (selectedAgents.Count() > 1)
            {
                // Находим максимальный приоритет среди выбранных элементов
                var maxPriority = selectedAgents.Max(agent => agent.Priority);
                NewPriorityTextBox.Text = maxPriority.ToString();
            }
            else
            {
                MessageBox.Show("Выберите хотя бы два элемента для изменения приоритета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close(); // Закрываем окно, если нет выбранных элементов
            }
        }

        private void ChangePriorityButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(NewPriorityTextBox.Text, out int newPriority))
            {
                // Обновляем приоритеты у выбранных элементов
                foreach (var selectedItem in _agentListView.SelectedItems)
                {
                    if (selectedItem is Agent agent)
                    {
                        if (newPriority > 0)
                            agent.Priority = newPriority;
                    }
                }

                // Обновляем привязку данных для отображения изменений
                if (newPriority > 0)
                {
                    MessageBox.Show("Успешно изменен приоритет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    _agentListView.Items.Refresh();
                    DialogResult = true;
                }
                else
                    MessageBox.Show("Введите коректный приоритет");
                
            }
            else
            {
                MessageBox.Show("Введите корректное числовое значение."); // Сообщение об ошибке
            }
        }
    }
}