using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartphoneDefectsDatabase
{
    public partial class MainForm : Form
    {
        private DefectContext dbContext;
        private MenuStrip menuStrip;
        private TabControl tabControl;

        public MainForm()
        {
            InitializeCustomComponents(); // Заменили на кастомный метод
            InitializeDatabase();
            InitializeMenu();
            InitializeTabs();
        }

        private void InitializeCustomComponents()
        {
            // Базовая настройка формы
            this.Text = "База данных обнаружения дефектов смартфонов";
            this.Size = new System.Drawing.Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeDatabase()
        {
            try
            {
                dbContext = new DefectContext();

                // Простая проверка подключения
                bool canConnect = dbContext.Database.Exists();

                if (canConnect)
                {
                    // Проверяем существование основных таблиц
                    CheckTablesExist();
                    MessageBox.Show("Подключение к БД успешно!", "Информация");
                }
                else
                {
                    MessageBox.Show("Не удалось подключиться к базе данных. Проверьте строку подключения.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД: {ex.Message}", "Ошибка");
            }
        }

        private void CheckTablesExist()
        {
            try
            {
                // Простая проверка - пытаемся получить count из таблиц
                var smartphoneCount = dbContext.Smartphones.Count();
                var roleCount = dbContext.Roles.Count();
                Console.WriteLine("Таблицы доступны");
            }
            catch
            {
                MessageBox.Show("Некоторые таблицы отсутствуют в базе данных. Выполните SQL скрипт для создания таблиц.", "Предупреждение");
            }
        }

        private void InitializeMenu()
        {
            menuStrip = new MenuStrip();

            var settingsMenu = new ToolStripMenuItem("Настройки");
            settingsMenu.DropDownItems.Add("Настройки подключения", null, SettingsConnection_Click);
            settingsMenu.DropDownItems.Add("Тест подключения", null, TestConnection_Click);

            var tablesMenu = new ToolStripMenuItem("Таблицы");
            tablesMenu.DropDownItems.Add("Партии", null, (s, e) => ShowTable("Parties"));
            tablesMenu.DropDownItems.Add("Смартфоны", null, (s, e) => ShowTable("Smartphones"));
            tablesMenu.DropDownItems.Add("Контроллеры", null, (s, e) => ShowTable("Controllers"));
            tablesMenu.DropDownItems.Add("Экраны", null, (s, e) => ShowTable("Screens"));
            tablesMenu.DropDownItems.Add("Дефекты", null, (s, e) => ShowTable("Defects"));
            tablesMenu.DropDownItems.Add("Изображения", null, (s, e) => ShowTable("Images"));

            var usersMenu = new ToolStripMenuItem("Пользователи");
            usersMenu.DropDownItems.Add("Роли", null, ShowRoles);
            usersMenu.DropDownItems.Add("Пользователи", null, ShowUsers);

            menuStrip.Items.AddRange(new[] { settingsMenu, tablesMenu, usersMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void InitializeTabs()
        {
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new System.Drawing.Point(0, menuStrip.Height);
            tabControl.Size = new System.Drawing.Size(this.Width, this.Height - menuStrip.Height);
            this.Controls.Add(tabControl);
        }

        private void SettingsConnection_Click(object sender, EventArgs e)
        {
            var settingsForm = new ConnectionSettingsForm();
            settingsForm.ShowDialog();
        }

        private void TestConnection_Click(object sender, EventArgs e)
        {
            TestConnection();
        }

        private void TestConnection()
        {
            try
            {
                using (var context = new DefectContext())
                {
                    context.Database.Connection.Open();
                    MessageBox.Show("Подключение к БД успешно!", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    context.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTable(string tableName)
        {
            // Устанавливаем заголовок окна
            this.Text = $"База данных дефектов - {tableName}";

            // Создаем или активируем вкладку для таблицы
            var existingTab = tabControl.TabPages.Cast<TabPage>()
                .FirstOrDefault(tp => tp.Text == tableName);

            if (existingTab == null)
            {
                var newTab = new TabPage(tableName);
                var tableControl = new TableControl(dbContext, tableName);
                tableControl.Dock = DockStyle.Fill;
                newTab.Controls.Add(tableControl);
                tabControl.TabPages.Add(newTab);
            }

            tabControl.SelectedTab = tabControl.TabPages.Cast<TabPage>()
                .FirstOrDefault(tp => tp.Text == tableName);
        }

        private void ShowRoles(object sender, EventArgs e)
        {
            this.Text = "База данных дефектов - Роли";
            var rolesForm = new RolesForm(dbContext);
            rolesForm.ShowDialog();
        }

        private void ShowUsers(object sender, EventArgs e)
        {
            this.Text = "База данных дефектов - Пользователи";
            var usersForm = new UsersForm(dbContext);
            usersForm.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            dbContext?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
