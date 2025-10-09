using System;
using System.Linq;
using System.Windows.Forms;

namespace SmartphoneDefectsDatabase
{
    public partial class RolesForm : Form
    {
        private DefectContext dbContext;
        private DataGridView dataGridView;
        private Button btnAdd, btnEdit, btnDelete;

        public RolesForm(DefectContext context)
        {
            dbContext = context;
            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Управление ролями";
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            // Панель кнопок
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            btnAdd = new Button { Text = "Добавить", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(100, 30) };
            btnEdit = new Button { Text = "Изменить", Location = new System.Drawing.Point(120, 10), Size = new System.Drawing.Size(100, 30) };
            btnDelete = new Button { Text = "Удалить", Location = new System.Drawing.Point(230, 10), Size = new System.Drawing.Size(100, 30) };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            panel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });
            this.Controls.Add(panel);

            // DataGridView
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
                dataGridView.DataSource = dbContext.Roles.ToList();
                dataGridView.Columns["RoleID"].Visible = false;
                dataGridView.Columns["Users"].Visible = false;

                // Настройка заголовков
                dataGridView.Columns["RoleName"].HeaderText = "Название роли";
                dataGridView.Columns["Description"].HeaderText = "Описание";
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
                var role = new Role();
                var editForm = new RoleEditForm(role);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    dbContext.Roles.Add(role);
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
                    var selectedRole = dataGridView.SelectedRows[0].DataBoundItem as Role;
                    var editForm = new RoleEditForm(selectedRole);
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
                MessageBox.Show("Выберите роль для редактирования");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRole = dataGridView.SelectedRows[0].DataBoundItem as Role;

                // Проверяем, есть ли пользователи с этой ролью
                var usersWithRole = dbContext.Users.Any(u => u.RoleID == selectedRole.RoleID);
                if (usersWithRole)
                {
                    MessageBox.Show("Невозможно удалить роль, так как есть пользователи с этой ролью.", "Ошибка");
                    return;
                }

                if (MessageBox.Show($"Удалить роль '{selectedRole.RoleName}'?", "Подтверждение",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        dbContext.Roles.Remove(selectedRole);
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
                MessageBox.Show("Выберите роль для удаления");
            }
        }
    }
}
