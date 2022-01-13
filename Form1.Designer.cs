
namespace WurmSermoner
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtOperator = new System.Windows.Forms.TextBox();
            this.txtLogsDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLogFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStuff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Operator";
            // 
            // txtOperator
            // 
            this.txtOperator.Location = new System.Drawing.Point(148, 56);
            this.txtOperator.Name = "txtOperator";
            this.txtOperator.Size = new System.Drawing.Size(126, 20);
            this.txtOperator.TabIndex = 1;
            this.txtOperator.Text = "Tiscattu";
            // 
            // txtLogsDir
            // 
            this.txtLogsDir.Location = new System.Drawing.Point(148, 82);
            this.txtLogsDir.Name = "txtLogsDir";
            this.txtLogsDir.Size = new System.Drawing.Size(586, 20);
            this.txtLogsDir.TabIndex = 3;
            this.txtLogsDir.Text = "D:\\Games\\Steam\\steamapps\\common\\Wurm Online\\gamedata\\players\\Tiscattu\\logs\\";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Logs dir";
            // 
            // txtLogFile
            // 
            this.txtLogFile.Location = new System.Drawing.Point(148, 108);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.Size = new System.Drawing.Size(126, 20);
            this.txtLogFile.TabIndex = 5;
            this.txtLogFile.Text = "_Event.2022-01.txt";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(94, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Logfile";
            // 
            // btnStuff
            // 
            this.btnStuff.Location = new System.Drawing.Point(257, 169);
            this.btnStuff.Name = "btnStuff";
            this.btnStuff.Size = new System.Drawing.Size(75, 23);
            this.btnStuff.TabIndex = 6;
            this.btnStuff.Text = "Stuff";
            this.btnStuff.UseVisualStyleBackColor = true;
            this.btnStuff.Click += new System.EventHandler(this.btnStuff_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnStuff);
            this.Controls.Add(this.txtLogFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLogsDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOperator);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form1";
            this.Text = "WurmSermoner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOperator;
        private System.Windows.Forms.TextBox txtLogsDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLogFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStuff;
    }
}

