namespace Updater
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.lblLocalVersion = new System.Windows.Forms.Label();
            this.lblReleaseVersion = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.linkLocal = new System.Windows.Forms.LinkLabel();
            this.linkRelease = new System.Windows.Forms.LinkLabel();
            this.btnOperation = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblLocalVersion
            // 
            this.lblLocalVersion.AutoSize = true;
            this.lblLocalVersion.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblLocalVersion.Location = new System.Drawing.Point(200, 56);
            this.lblLocalVersion.Name = "lblLocalVersion";
            this.lblLocalVersion.Size = new System.Drawing.Size(55, 21);
            this.lblLocalVersion.TabIndex = 2;
            this.lblLocalVersion.Text = "1.0.0.0";
            // 
            // lblReleaseVersion
            // 
            this.lblReleaseVersion.AutoSize = true;
            this.lblReleaseVersion.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblReleaseVersion.Location = new System.Drawing.Point(200, 96);
            this.lblReleaseVersion.Name = "lblReleaseVersion";
            this.lblReleaseVersion.Size = new System.Drawing.Size(55, 21);
            this.lblReleaseVersion.TabIndex = 4;
            this.lblReleaseVersion.Text = "1.0.0.0";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblName.Location = new System.Drawing.Point(16, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(98, 25);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Название";
            // 
            // linkLocal
            // 
            this.linkLocal.AutoSize = true;
            this.linkLocal.Location = new System.Drawing.Point(16, 56);
            this.linkLocal.Name = "linkLocal";
            this.linkLocal.Size = new System.Drawing.Size(170, 21);
            this.linkLocal.TabIndex = 1;
            this.linkLocal.TabStop = true;
            this.linkLocal.Text = "Установленная версия";
            this.linkLocal.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLocal_LinkClicked);
            // 
            // linkRelease
            // 
            this.linkRelease.AutoSize = true;
            this.linkRelease.Location = new System.Drawing.Point(16, 96);
            this.linkRelease.Name = "linkRelease";
            this.linkRelease.Size = new System.Drawing.Size(140, 21);
            this.linkRelease.TabIndex = 3;
            this.linkRelease.TabStop = true;
            this.linkRelease.Text = "Последняя версия";
            this.linkRelease.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkRelease_LinkClicked);
            // 
            // btnOperation
            // 
            this.btnOperation.Location = new System.Drawing.Point(16, 144);
            this.btnOperation.Name = "btnOperation";
            this.btnOperation.Size = new System.Drawing.Size(104, 32);
            this.btnOperation.TabIndex = 5;
            this.btnOperation.Text = "check";
            this.btnOperation.UseVisualStyleBackColor = true;
            this.btnOperation.Click += new System.EventHandler(this.BtnOperation_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(216, 144);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 32);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 193);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOperation);
            this.Controls.Add(this.linkRelease);
            this.Controls.Add(this.linkLocal);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblReleaseVersion);
            this.Controls.Add(this.lblLocalVersion);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Обновление";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLocalVersion;
        private System.Windows.Forms.Label lblReleaseVersion;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.LinkLabel linkLocal;
        private System.Windows.Forms.LinkLabel linkRelease;
        private System.Windows.Forms.Button btnOperation;
        private System.Windows.Forms.Button btnClose;
    }
}

