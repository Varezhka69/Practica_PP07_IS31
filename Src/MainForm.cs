using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartphoneDefectsDatabase1
{
    public partial class MainForm : Form
    {
        private DefectContext dbContext;
        private MenuStrip menuStrip;
        private TabControl tabControl;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeMenu();
            InitializeTabs();
        }

        private void InitializeDatabase()
        {
            try
            {
                dbContext = new DefectContext();

                if (!dbContext.Database.Exists() || ShouldRecreateDatabase())
                {
                    if (dbContext.Database.Exists())
                    {
                        dbContext.Database.Delete();
                    }

                    dbContext.Database.Create();

                    SeedInitialData();

                    MessageBox.Show("База данных создана. Логин: admin, Пароль: admin123", "Информация");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}\n\nРекомендуется удалить существующую базу данных и запустить приложение заново.", "Ошибка");
            }
        }

        private bool ShouldRecreateDatabase()
        {
            try
            {
                var testRoles = dbContext.Roles.Any();
                var testUsers = dbContext.Users.Any();
                return false;
            }
            catch
            {
                return true;
            }
        }

        private void SeedInitialData()
        {
            var adminRole = new Role { RoleName = "Администратор", Description = "Полный доступ ко всем функциям" };
            var userRole = new Role { RoleName = "Пользователь", Description = "Базовый доступ" };
            var inspectorRole = new Role { RoleName = "Инспектор", Description = "Доступ к проверке дефектов" };

            dbContext.Roles.Add(adminRole);
            dbContext.Roles.Add(userRole);
            dbContext.Roles.Add(inspectorRole);

            var adminUser = new User
            {
                Username = "admin",
                Password = "admin123",
                FullName = "Администратор системы",
                Email = "admin@system.com",
                Role = adminRole
            };

            dbContext.Users.Add(adminUser);

            dbContext.SaveChanges();
        }

        private void InitializeMenu()
        {
            menuStrip = new MenuStrip();

            var settingsMenu = new ToolStripMenuItem("Настройки");
            settingsMenu.DropDownItems.Add("Настройки подключения", null, SettingsConnection_Click);
            settingsMenu.DropDownItems.Add("Тест подключения", null, TestConnection_Click);

            var tablesMenu = new ToolStripMenuItem("Таблицы");
            tablesMenu.DropDownItems.Add("Смартфоны", null, (s, e) => ShowTable("Smartphones"));
            tablesMenu.DropDownItems.Add("Экраны", null, (s, e) => ShowTable("Screens"));
            tablesMenu.DropDownItems.Add("Дефекты", null, (s, e) => ShowTable("Defects"));
            tablesMenu.DropDownItems.Add("Партии", null, (s, e) => ShowTable("Party"));
            tablesMenu.DropDownItems.Add("Контроллеры", null, (s, e) => ShowTable("Controllers"));
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
            this.Text = $"База данных дефектов - {tableName}";

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
