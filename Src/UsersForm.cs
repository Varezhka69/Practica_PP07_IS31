using System;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity;

namespace SmartphoneDefectsDatabase
{
    public partial class UsersForm : Form
    {
        private DefectContext dbContext;
        private DataGridView dataGridView;
        private Button btnAdd, btnEdit, btnDelete, btnChangePassword;

        public UsersForm(DefectContext context)
        {
            dbContext = context;
            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Управление пользователями";
            this.Size = new System.Drawing.Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            btnAdd = new Button { Text = "Добавить", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(100, 30) };
            btnEdit = new Button { Text = "Изменить", Location = new System.Drawing.Point(120, 10), Size = new System.Drawing.Size(100, 30) };
            btnDelete = new Button { Text = "Удалить", Location = new System.Drawing.Point(230, 10), Size = new System.Drawing.Size(100, 30) };
            btnChangePassword = new Button { Text = "Сменить пароль", Location = new System.Drawing.Point(340, 10), Size = new System.Drawing.Size(120, 30) };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnChangePassword.Click += BtnChangePassword_Click;

            panel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnChangePassword });
            this.Controls.Add(panel);

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Location = new System.Drawing.Point(0, 50),
                Size = new System.Drawing.Size(this.Width, this.Height - 50),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            this.Controls.Add(dataGridView);
        }

        private void LoadData()
        {
            try
            {
                dataGridView.DataSource = dbContext.Users.Include(u => u.Role).ToList();
                dataGridView.Columns["UserID"].Visible = false;
                dataGridView.Columns["Password"].Visible = false;
                dataGridView.Columns["RoleID"].Visible = false;
                dataGridView.Columns["Role"].Visible = false;

                // Настройка заголовков
                dataGridView.Columns["Username"].HeaderText = "Логин";
                dataGridView.Columns["FullName"].HeaderText = "ФИО";
                dataGridView.Columns["Email"].HeaderText = "Email";
                dataGridView.Columns["IsActive"].HeaderText = "Активен";
                dataGridView.Columns["Role.RoleName"].HeaderText = "Роль";
                dataGridView.Columns["CreatedDate"].HeaderText = "Дата создания";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var user = new User();
                var editForm = new UserEditForm(dbContext, user);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedUser = dataGridView.SelectedRows[0].DataBoundItem as User;
                    var editForm = new UserEditForm(dbContext, selectedUser);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        dbContext.SaveChanges();
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при редактировании: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedUser = dataGridView.SelectedRows[0].DataBoundItem as User;

                if (MessageBox.Show($"Удалить пользователя '{selectedUser.Username}'?", "Подтверждение",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        dbContext.Users.Remove(selectedUser);
                        dbContext.SaveChanges();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления");
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedUser = dataGridView.SelectedRows[0].DataBoundItem as User;
                var passwordForm = new ChangePasswordForm(selectedUser);
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    dbContext.SaveChanges();
                    MessageBox.Show("Пароль успешно изменен");
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для смены пароля");
            }
        }
    }
}
