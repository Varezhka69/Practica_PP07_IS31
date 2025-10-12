using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartphoneDefectsDatabase1
{
    public partial class TableControl : UserControl
    {
        private DefectContext dbContext;
        private string tableName;
        private DataGridView dataGridView;
        private Button btnAdd, btnEdit, btnDelete;

        public TableControl(DefectContext context, string table)
        {
            dbContext = context;
            tableName = table;
            InitializeCustomComponent();
            LoadData();
        }

        private void InitializeCustomComponent()
        {
            this.Size = new System.Drawing.Size(1000, 700);

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersWidth = 50,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F),
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 28 },
                Location = new System.Drawing.Point(0, 50),
                Size = new System.Drawing.Size(this.Width, this.Height - 50)
            };

            this.Controls.Add(dataGridView);
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = SystemColors.Control
            };

            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new System.Drawing.Point(10, 12),
                Size = new System.Drawing.Size(100, 30),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };

            btnEdit = new Button
            {
                Text = "Изменить",
                Location = new System.Drawing.Point(120, 12),
                Size = new System.Drawing.Size(100, 30),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };

            btnDelete = new Button
            {
                Text = "Удалить",
                Location = new System.Drawing.Point(230, 12),
                Size = new System.Drawing.Size(100, 30),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            panel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });
            this.Controls.Add(panel);
        }

        private void LoadData()
        {
            try
            {
                switch (tableName)
                {
                    case "Smartphones":
                        dataGridView.DataSource = dbContext.Smartphones.ToList();
                        break;
                    case "Screens":
                        dataGridView.DataSource = dbContext.Screens.Include(s => s.Smartphone).ToList();
                        break;
                    case "Defects":
                        dataGridView.DataSource = dbContext.Defects.Include(d => d.Screen).ToList();
                        break;
                    case "Parties":
                        dataGridView.DataSource = dbContext.Parties.ToList();
                        break;
                    case "Controllers":
                        dataGridView.DataSource = dbContext.Controllers.ToList();
                        break;
                    case "Images":
                        dataGridView.DataSource = dbContext.Images.Include(i => i.Defect).ToList();
                        break;
                    default:
                        MessageBox.Show($"Неизвестная таблица: {tableName}");
                        break;
                }

                dataGridView.Refresh();
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
                var editForm = new EditForm(dbContext, tableName);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
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
                    var selectedItem = dataGridView.SelectedRows[0].DataBoundItem;
                    var editForm = new EditForm(dbContext, tableName, selectedItem);
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
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Удалить запись?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        var selectedItem = dataGridView.SelectedRows[0].DataBoundItem;
                        dbContext.Entry(selectedItem).State = EntityState.Deleted;
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
                MessageBox.Show("Выберите запись для удаления");
            }
        }
    }
}
