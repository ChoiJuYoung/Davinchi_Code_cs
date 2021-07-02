namespace Davinchi_Code
{
    // VB : C# = 15 : 1
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.lblNotify = new System.Windows.Forms.Label();
            this.picTitle = new System.Windows.Forms.PictureBox();
            this.CmdSingle = new System.Windows.Forms.Button();
            this.CmdMulti = new System.Windows.Forms.Button();
            this.CmdExit = new System.Windows.Forms.Button();
            this.timCheck = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNotify
            // 
            this.lblNotify.Location = new System.Drawing.Point(0, 168);
            this.lblNotify.Name = "lblNotify";
            this.lblNotify.Size = new System.Drawing.Size(674, 25);
            this.lblNotify.TabIndex = 0;
            this.lblNotify.Text = "Server의 상태를 확인중입니다 ...";
            this.lblNotify.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picTitle
            // 
            this.picTitle.Image = ((System.Drawing.Image)(resources.GetObject("picTitle.Image")));
            this.picTitle.Location = new System.Drawing.Point(0, 32);
            this.picTitle.Name = "picTitle";
            this.picTitle.Size = new System.Drawing.Size(674, 112);
            this.picTitle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picTitle.TabIndex = 1;
            this.picTitle.TabStop = false;
            // 
            // CmdSingle
            // 
            this.CmdSingle.BackColor = System.Drawing.Color.SkyBlue;
            this.CmdSingle.Enabled = false;
            this.CmdSingle.FlatAppearance.BorderColor = System.Drawing.Color.Maroon;
            this.CmdSingle.Location = new System.Drawing.Point(72, 224);
            this.CmdSingle.Name = "CmdSingle";
            this.CmdSingle.Size = new System.Drawing.Size(521, 49);
            this.CmdSingle.TabIndex = 2;
            this.CmdSingle.Text = "혼자 즐기기 (현재 미지원 상태입니다.)";
            this.CmdSingle.UseVisualStyleBackColor = false;
            // 
            // CmdMulti
            // 
            this.CmdMulti.BackColor = System.Drawing.Color.Violet;
            this.CmdMulti.Enabled = false;
            this.CmdMulti.Location = new System.Drawing.Point(72, 280);
            this.CmdMulti.Name = "CmdMulti";
            this.CmdMulti.Size = new System.Drawing.Size(521, 49);
            this.CmdMulti.TabIndex = 3;
            this.CmdMulti.Text = "여러명이 즐기기 (현재 구현 : 최대 4인용)";
            this.CmdMulti.UseVisualStyleBackColor = false;
            this.CmdMulti.Click += new System.EventHandler(this.CmdMulti_Click);
            // 
            // CmdExit
            // 
            this.CmdExit.BackColor = System.Drawing.Color.SlateGray;
            this.CmdExit.Location = new System.Drawing.Point(72, 336);
            this.CmdExit.Name = "CmdExit";
            this.CmdExit.Size = new System.Drawing.Size(521, 49);
            this.CmdExit.TabIndex = 4;
            this.CmdExit.Text = "정말 아쉽고 마음이 텅 빈 것처럼 느껴지지만 그러한 마음을 뒤로 하고 종료하기";
            this.CmdExit.UseVisualStyleBackColor = false;
            this.CmdExit.Click += new System.EventHandler(this.CmdExit_Click);
            // 
            // timCheck
            // 
            this.timCheck.Enabled = true;
            this.timCheck.Interval = 300;
            this.timCheck.Tick += new System.EventHandler(this.timCheck_Tick);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(674, 419);
            this.Controls.Add(this.CmdExit);
            this.Controls.Add(this.CmdMulti);
            this.Controls.Add(this.CmdSingle);
            this.Controls.Add(this.picTitle);
            this.Controls.Add(this.lblNotify);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Made By 오지석, 최주영 in UOS Computer Science.";
            ((System.ComponentModel.ISupportInitialize)(this.picTitle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNotify;
        private System.Windows.Forms.PictureBox picTitle;
        private System.Windows.Forms.Button CmdSingle;
        private System.Windows.Forms.Button CmdMulti;
        private System.Windows.Forms.Button CmdExit;
        private System.Windows.Forms.Timer timCheck;

    }
}

