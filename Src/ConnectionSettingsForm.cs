using System;
using System.Windows.Forms;
using System.Configuration;

namespace SmartphoneDefectsDatabase1
{
    public partial class ConnectionSettingsForm : Form
    {
        private TextBox txtServer, txtDatabase, txtUser, txtPassword;

        public ConnectionSettingsForm()
        {
            InitializeCustomComponents();
            LoadCurrentSettings();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Настройки подключения к БД";
            this.Size = new System.Drawing.Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(20),
                AutoSize = true
            };

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            for (int i = 0; i < 4; i++)
            {
                mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            }
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            mainPanel.Controls.Add(new Label
            {
                Text = "Сервер:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            }, 0, 0);

            txtServer = new TextBox
            {
                Name = "txtServer",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            mainPanel.Controls.Add(txtServer, 1, 0);

            mainPanel.Controls.Add(new Label
            {
                Text = "База данных:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            }, 0, 1);

            txtDatabase = new TextBox
            {
                Name = "txtDatabase",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            mainPanel.Controls.Add(txtDatabase, 1, 1);

            mainPanel.Controls.Add(new Label
            {
                Text = "Пользователь:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            }, 0, 2);

            txtUser = new TextBox
            {
                Name = "txtUser",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            mainPanel.Controls.Add(txtUser, 1, 2);

            mainPanel.Controls.Add(new Label
            {
                Text = "Пароль:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            }, 0, 3);

            txtPassword = new TextBox
            {
                Name = "txtPassword",
                Dock = DockStyle.Fill,
                UseSystemPasswordChar = true,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            mainPanel.Controls.Add(txtPassword, 1, 3);

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(20)
            };

            var btnTest = new Button
            {
                Text = "Тест подключения",
                Size = new System.Drawing.Size(120, 30),
                Location = new System.Drawing.Point(150, 15),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };

            var btnSave = new Button
            {
                Text = "Сохранить",
                Size = new System.Drawing.Size(80, 30),
                Location = new System.Drawing.Point(280, 15),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                Size = new System.Drawing.Size(80, 30),
                Location = new System.Drawing.Point(370, 15),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };

            btnTest.Click += BtnTest_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            buttonPanel.Controls.AddRange(new Control[] { btnTest, btnSave, btnCancel });

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["DefectConnection"]?.ConnectionString;

                if (!string.IsNullOrEmpty(connectionString))
                {
                    if (connectionString.Contains("Data Source"))
                    {
                        var parts = connectionString.Split(';');
                        foreach (var part in parts)
                        {
                            if (part.StartsWith("Data Source"))
                                txtServer.Text = part.Substring(12);
                            else if (part.StartsWith("Initial Catalog"))
                                txtDatabase.Text = part.Substring(16);
                            else if (part.StartsWith("User Id"))
                                txtUser.Text = part.Substring(8);
                            else if (part.StartsWith("Password"))
                                txtPassword.Text = part.Substring(9);
                        }
                    }

                    if (string.IsNullOrEmpty(txtServer.Text))
                        txtServer.Text = ".\\SQLEXPRESS";
                    if (string.IsNullOrEmpty(txtDatabase.Text))
                        txtDatabase.Text = "SmartphoneDefectsDB";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtServer.Text = ".\\SQLEXPRESS";
                txtDatabase.Text = "SmartphoneDefectsDB";
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            try
            {
                string testConnectionString = BuildConnectionString();

                using (var connection = new System.Data.SqlClient.SqlConnection(testConnectionString))
                {
                    connection.Open();
                    MessageBox.Show("Подключение к БД успешно установлено!", "Тест подключения",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка подключения",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtServer.Text))
                {
                    MessageBox.Show("Введите имя сервера", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtServer.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDatabase.Text))
                {
                    MessageBox.Show("Введите имя базы данных", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDatabase.Focus();
                    return;
                }

                SaveConnectionString();

                MessageBox.Show("Настройки подключения сохранены успешно!\nПерезапустите приложение для применения изменений.",
                    "Сохранено", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildConnectionString()
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                return $"Data Source={txtServer.Text};Initial Catalog={txtDatabase.Text};Integrated Security=True;MultipleActiveResultSets=True";
            }
            else
            {
                return $"Data Source={txtServer.Text};Initial Catalog={txtDatabase.Text};User Id={txtUser.Text};Password={txtPassword.Text};MultipleActiveResultSets=True";
            }
        }

        private void SaveConnectionString()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStrings = config.ConnectionStrings.ConnectionStrings;

                string newConnectionString = BuildConnectionString();

                if (connectionStrings["DefectConnection"] != null)
                {
                    connectionStrings["DefectConnection"].ConnectionString = newConnectionString;
                }
                else
                {
                    connectionStrings.Add(new ConnectionStringSettings("DefectConnection", newConnectionString));
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось сохранить настройки в config файл: {ex.Message}");
            }
        }
    }
}
