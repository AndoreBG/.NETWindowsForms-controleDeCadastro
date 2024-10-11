using OfficeOpenXml;
using OfficeOpenXml.Packaging.Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace controleDeCadastro
{
    public partial class Form1 : Form
    {
        DataTable produtosTable = new DataTable();

        public Form1()
        {
            InitializeComponent();

            // Estrutura inicial do produtosTable
            produtosTable.Columns.Add("ID", typeof(int));
            produtosTable.Columns.Add("Nome", typeof(string));
            produtosTable.Columns.Add("Preco", typeof(double));
            produtosTable.Columns.Add("Qtd", typeof(int));

            // Vincular o DataTable ao DataGridView
            dgvProdutos.DataSource = produtosTable;

            // Chave primária (Id) automática
            produtosTable.PrimaryKey = new DataColumn[] { produtosTable.Columns["ID"] };

            // Desabilitar a adição automática de uma linha vazia
            dgvProdutos.AllowUserToAddRows = false;

            // Adicionar valores predefinidos
            produtosTable.Rows.Add(1, "Produto A", 10.0, 5);
            produtosTable.Rows.Add(2, "Produto B", 15.0, 8);
            produtosTable.Rows.Add(3, "Produto C", 20.0, 3);
        }

        // Adicionar dados
        private void btnAdc_Click(object sender, EventArgs e)
        {
            // Verificar se todos os campos estão preenchidos
            if (string.IsNullOrWhiteSpace(txbNome.Text) ||
                string.IsNullOrWhiteSpace(txbPreco.Text) ||
                string.IsNullOrWhiteSpace(txbQtd.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos antes de adicionar.", "Campos Vazios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Interrompe o processo de adição caso algum campo esteja vazio
            }

            try
            {
                // Gerar um novo ID automático
                int novoID = produtosTable.Rows.Count > 0 ? Convert.ToInt32(produtosTable.Compute("MAX(ID)", "")) + 1 : 1;

                // Adicionar a nova linha ao DataTable
                produtosTable.Rows.Add(novoID, txbNome.Text, Convert.ToDouble(txbPreco.Text), Convert.ToInt32(txbQtd.Text));

                // Limpar os campos de texto
                txbNome.Clear();
                txbPreco.Clear();
                txbQtd.Clear();
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, insira valores numéricos válidos para o preço e quantidade.", "Erro de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.SelectedRows.Count > 0)
            {
                // Linha selecionada
                int selectedID = Convert.ToInt32(dgvProdutos.SelectedRows[0].Cells["ID"].Value);

                // Encontrar a linha correspondente no DataTable
                DataRow row = produtosTable.Rows[selectedID - 1];

                if (row != null)
                {
                    // Atualizar os valores
                    row["Nome"] = txbNome.Text;
                    row["Preco"] = Convert.ToDouble(txbPreco.Text);
                    row["Qtd"] = Convert.ToInt32(txbQtd.Text);
                }

                // Limpar os campos de texto
                txbNome.Clear();
                txbPreco.Clear();
                txbQtd.Clear();
            }
            else
            {
                MessageBox.Show("Selecione um produto para editar.");
            }
        }

        // Remover dados
        private void btnRemov_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.SelectedRows.Count > 0)
            {
                // Linha selecionada
                int selectedID = Convert.ToInt32(dgvProdutos.SelectedRows[0].Cells["ID"].Value);

                // Encontrar e remover a linha correspondente no DataTable
                DataRow row = produtosTable.Rows.Find(selectedID);
                if (row != null)
                {
                    produtosTable.Rows.Remove(row);
                }

                // Limpar os campos de texto
                txbNome.Clear();
                txbPreco.Clear();
                txbQtd.Clear();
            }
            else
            {
                MessageBox.Show("Selecione um produto para editar.");
            }
        }

        // Selecionar dados no DataGridView para editar
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProdutos.SelectedRows.Count > 0)
            {
                // Preencher os campos de texto com os dados selecionados
                txbNome.Text = dgvProdutos.SelectedRows[0].Cells["Nome"].Value.ToString();
                txbPreco.Text = dgvProdutos.SelectedRows[0].Cells["Preco"].Value.ToString();
                txbQtd.Text = dgvProdutos.SelectedRows[0].Cells["Qtd"].Value.ToString();
                txbID.Text = dgvProdutos.SelectedRows[0].Cells["ID"].Value.ToString();
            }
        }

        private void btnExpor_Click(object sender, EventArgs e)
        {
            // Configurar o SaveFileDialog para salvar arquivos .xlsx
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx"; // Filtro para arquivos Excel
            saveFileDialog.Title = "Salvar arquivo Excel";
            saveFileDialog.FileName = "dados.xlsx"; // Nome padrão do arquivo

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Criar um novo pacote Excel
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        // Adicionar uma nova planilha ao pacote
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Dados");

                        // Escrever os cabeçalhos (nomes das colunas)
                        for (int i = 0; i < dgvProdutos.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = dgvProdutos.Columns[i].HeaderText;
                            worksheet.Cells[1, i + 1].Style.Font.Bold = true; // Cabeçalhos em negrito (opcional)
                        }

                        // Escrever os dados das células
                        for (int i = 0; i < dgvProdutos.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgvProdutos.Columns.Count; j++)
                            {
                                worksheet.Cells[i + 2, j + 1].Value = dgvProdutos.Rows[i].Cells[j].Value?.ToString();
                            }
                        }

                        // Ajustar a largura das colunas automaticamente
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Salvar o arquivo no local escolhido pelo usuário
                        FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
                        excelPackage.SaveAs(fileInfo);
                    }

                    MessageBox.Show("Dados exportados com sucesso!", "Exportar Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar o arquivo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {    // Configurar o OpenFileDialog para permitir ao usuário escolher o arquivo .xlsx
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xlsx";
            openFileDialog.Title = "Selecione o arquivo Excel";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Criar um novo FileInfo para o arquivo Excel selecionado
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                    // Usar o EPPlus para abrir o arquivo Excel
                    using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
                    {
                        // Pegar a primeira planilha do arquivo Excel
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];

                        // Limpar o DataTable antes de carregar novos dados
                        produtosTable.Clear();

                        // Remover a chave primária antes de limpar as colunas
                        if (produtosTable.PrimaryKey.Length > 0)
                        {
                            produtosTable.PrimaryKey = null; // Remover a chave primária
                        }

                        // Adicionar colunas ao DataTable
                        produtosTable.Columns.Clear();
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            produtosTable.Columns.Add(worksheet.Cells[1, col].Text); // Cabeçalhos da planilha
                        }

                        // Ler as linhas e preencher o DataTable
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            DataRow dataRow = produtosTable.NewRow();

                            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                            {
                                dataRow[col - 1] = worksheet.Cells[row, col].Text;
                            }

                            produtosTable.Rows.Add(dataRow);
                        }

                        // Reaplicar a chave primária (assumindo que a primeira coluna seja o "ID")
                        if (produtosTable.Columns.Contains("ID")) // Verificar se a coluna "ID" existe
                        {
                            produtosTable.PrimaryKey = new DataColumn[] { produtosTable.Columns["ID"] };
                        }

                        // Vincular o DataTable ao DataGridView para exibir os dados
                        dgvProdutos.DataSource = produtosTable;
                    }

                    MessageBox.Show("Dados importados com sucesso!", "Importar Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao importar o arquivo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
