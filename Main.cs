using System;
using System.IO;
using System.Windows.Forms;

namespace FoxNote
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string note = txtNote.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(note) || string.IsNullOrWhiteSpace(password))
            {
                lblStatus.Text = "⚠️ Please enter text and password.";
                return;
            }

            try
            {
                var encrypted = CryptoHelper.Encrypt(note, password);
                txtNote.Text = Convert.ToBase64String(encrypted);
                lblStatus.Text = "✅ The note was encrypted.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Encryption error: " + ex.Message;
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            string base64 = txtNote.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(base64) || string.IsNullOrWhiteSpace(password))
            {
                lblStatus.Text = "⚠️ Please enter the ciphertext and password.";
                return;
            }

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(base64);
                string decrypted = CryptoHelper.Decrypt(cipherBytes, password);
                txtNote.Text = decrypted;
                lblStatus.Text = "🔓 Decryption was successful.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Wrong password or decoding error";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string note = txtNote.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(note) || string.IsNullOrWhiteSpace(password))
            {
                lblStatus.Text = "⚠️ Password and note are required.";
                return;
            }

            try
            {
                byte[] encrypted = CryptoHelper.Encrypt(note, password);

                SaveFileDialog save = new SaveFileDialog
                {
                    Filter = "FoxNote Encrypted File (*.fnote)|*.fnote",
                    Title = "Save encrypted notes",
                    FileName = "note.fnote"
                };

                if (save.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(save.FileName, encrypted);
                    lblStatus.Text = "✅ File saved successfully.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Error saving: " + ex.Message;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "FoxNote Encrypted File (*.fnote)|*.fnote";
            open.Title = "Open encrypted file";

            if (open.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    byte[] fileData = File.ReadAllBytes(open.FileName);
                    string password = txtPassword.Text;

                    if (string.IsNullOrWhiteSpace(password))
                    {
                        lblStatus.Text = "⚠️ Enter password.";
                        return;
                    }

                    string decrypted = CryptoHelper.Decrypt(fileData, password);
                    txtNote.Text = decrypted;
                    lblStatus.Text = "🔓 Note opened successfully.";
                }
                catch
                {
                    lblStatus.Text = "❌The password is incorrect or the file is corrupt.";
                }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;

        }

        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPass.Checked;
        }
    }
}
