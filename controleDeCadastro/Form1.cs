using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
}
