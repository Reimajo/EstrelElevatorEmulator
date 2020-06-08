namespace WindowsFormsApp1
{
    partial class ElevatorEmulator
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonCallDown_0 = new System.Windows.Forms.Button();
            this.elevator1 = new System.Windows.Forms.TextBox();
            this.elevator2 = new System.Windows.Forms.TextBox();
            this.elevator3 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonCallUp_0 = new System.Windows.Forms.Button();
            this.textBoxElevator1OpenReception = new System.Windows.Forms.TextBox();
            this.textBoxElevator2OpenReception = new System.Windows.Forms.TextBox();
            this.textBoxElevator3OpenReception = new System.Windows.Forms.TextBox();
            this.buttonCallUp_5 = new System.Windows.Forms.Button();
            this.buttonCallDown_5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonCallUp_13 = new System.Windows.Forms.Button();
            this.buttonCallDown_13 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxElevator3Open = new System.Windows.Forms.TextBox();
            this.textBoxElevator2Open = new System.Windows.Forms.TextBox();
            this.textBoxElevator1Open = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCallDown_0
            // 
            this.buttonCallDown_0.BackColor = System.Drawing.Color.LightGray;
            this.buttonCallDown_0.Location = new System.Drawing.Point(7, 68);
            this.buttonCallDown_0.Name = "buttonCallDown_0";
            this.buttonCallDown_0.Size = new System.Drawing.Size(100, 36);
            this.buttonCallDown_0.TabIndex = 0;
            this.buttonCallDown_0.Text = "DOWN";
            this.buttonCallDown_0.UseVisualStyleBackColor = false;
            this.buttonCallDown_0.Click += new System.EventHandler(this.buttonCallDown_0_Click);
            // 
            // elevator1
            // 
            this.elevator1.Location = new System.Drawing.Point(41, 57);
            this.elevator1.Name = "elevator1";
            this.elevator1.Size = new System.Drawing.Size(100, 20);
            this.elevator1.TabIndex = 1;
            // 
            // elevator2
            // 
            this.elevator2.Location = new System.Drawing.Point(234, 57);
            this.elevator2.Name = "elevator2";
            this.elevator2.Size = new System.Drawing.Size(100, 20);
            this.elevator2.TabIndex = 2;
            // 
            // elevator3
            // 
            this.elevator3.Location = new System.Drawing.Point(425, 57);
            this.elevator3.Name = "elevator3";
            this.elevator3.Size = new System.Drawing.Size(100, 20);
            this.elevator3.TabIndex = 3;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonCallUp_0
            // 
            this.buttonCallUp_0.BackColor = System.Drawing.Color.LightGray;
            this.buttonCallUp_0.Location = new System.Drawing.Point(7, 26);
            this.buttonCallUp_0.Name = "buttonCallUp_0";
            this.buttonCallUp_0.Size = new System.Drawing.Size(100, 36);
            this.buttonCallUp_0.TabIndex = 4;
            this.buttonCallUp_0.Text = "UP";
            this.buttonCallUp_0.UseVisualStyleBackColor = false;
            this.buttonCallUp_0.Click += new System.EventHandler(this.buttonCallUp_0_Click);
            // 
            // textBoxElevator1OpenReception
            // 
            this.textBoxElevator1OpenReception.Location = new System.Drawing.Point(12, 116);
            this.textBoxElevator1OpenReception.Name = "textBoxElevator1OpenReception";
            this.textBoxElevator1OpenReception.Size = new System.Drawing.Size(159, 20);
            this.textBoxElevator1OpenReception.TabIndex = 5;
            // 
            // textBoxElevator2OpenReception
            // 
            this.textBoxElevator2OpenReception.Location = new System.Drawing.Point(195, 116);
            this.textBoxElevator2OpenReception.Name = "textBoxElevator2OpenReception";
            this.textBoxElevator2OpenReception.Size = new System.Drawing.Size(177, 20);
            this.textBoxElevator2OpenReception.TabIndex = 6;
            // 
            // textBoxElevator3OpenReception
            // 
            this.textBoxElevator3OpenReception.Location = new System.Drawing.Point(392, 116);
            this.textBoxElevator3OpenReception.Name = "textBoxElevator3OpenReception";
            this.textBoxElevator3OpenReception.Size = new System.Drawing.Size(168, 20);
            this.textBoxElevator3OpenReception.TabIndex = 7;
            // 
            // buttonCallUp_5
            // 
            this.buttonCallUp_5.BackColor = System.Drawing.Color.LightGray;
            this.buttonCallUp_5.Location = new System.Drawing.Point(229, 275);
            this.buttonCallUp_5.Name = "buttonCallUp_5";
            this.buttonCallUp_5.Size = new System.Drawing.Size(100, 36);
            this.buttonCallUp_5.TabIndex = 9;
            this.buttonCallUp_5.Text = "UP";
            this.buttonCallUp_5.UseVisualStyleBackColor = false;
            this.buttonCallUp_5.Click += new System.EventHandler(this.buttonCallUp_5_Click);
            // 
            // buttonCallDown_5
            // 
            this.buttonCallDown_5.BackColor = System.Drawing.Color.LightGray;
            this.buttonCallDown_5.Location = new System.Drawing.Point(229, 317);
            this.buttonCallDown_5.Name = "buttonCallDown_5";
            this.buttonCallDown_5.Size = new System.Drawing.Size(100, 36);
            this.buttonCallDown_5.TabIndex = 8;
            this.buttonCallDown_5.Text = "DOWN";
            this.buttonCallDown_5.UseVisualStyleBackColor = false;
            this.buttonCallDown_5.Click += new System.EventHandler(this.buttonCallDown_5_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Elevator 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(257, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Elevator 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(448, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Elevator 3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "floor";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(201, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "floor";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(389, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "floor";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Reception buttons";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(235, 259);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Floor 5 buttons";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(431, 259);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Floor 13 buttons";
            // 
            // buttonCallUp_13
            // 
            this.buttonCallUp_13.BackColor = System.Drawing.Color.LightGray;
            this.buttonCallUp_13.Location = new System.Drawing.Point(425, 275);
            this.buttonCallUp_13.Name = "buttonCallUp_13";
            this.buttonCallUp_13.Size = new System.Drawing.Size(100, 36);
            this.buttonCallUp_13.TabIndex = 19;
            this.buttonCallUp_13.Text = "UP";
            this.buttonCallUp_13.UseVisualStyleBackColor = false;
            this.buttonCallUp_13.Click += new System.EventHandler(this.buttonCallUp_13_Click);
            // 
            // buttonCallDown_13
            // 
            this.buttonCallDown_13.BackColor = System.Drawing.Color.LightGray;
            this.buttonCallDown_13.Location = new System.Drawing.Point(425, 317);
            this.buttonCallDown_13.Name = "buttonCallDown_13";
            this.buttonCallDown_13.Size = new System.Drawing.Size(100, 36);
            this.buttonCallDown_13.TabIndex = 18;
            this.buttonCallDown_13.Text = "DOWN";
            this.buttonCallDown_13.UseVisualStyleBackColor = false;
            this.buttonCallDown_13.Click += new System.EventHandler(this.buttonCallDown_13_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(38, 100);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "current state reception";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(226, 100);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(113, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "current state reception";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(422, 100);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(113, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "current state reception";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.buttonCallDown_0);
            this.panel1.Controls.Add(this.buttonCallUp_0);
            this.panel1.Location = new System.Drawing.Point(25, 250);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(116, 116);
            this.panel1.TabIndex = 24;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(448, 172);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(66, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "current state";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(257, 172);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 13);
            this.label14.TabIndex = 29;
            this.label14.Text = "current state";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(54, 172);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(66, 13);
            this.label15.TabIndex = 28;
            this.label15.Text = "current state";
            // 
            // textBoxElevator3Open
            // 
            this.textBoxElevator3Open.Location = new System.Drawing.Point(392, 188);
            this.textBoxElevator3Open.Name = "textBoxElevator3Open";
            this.textBoxElevator3Open.Size = new System.Drawing.Size(168, 20);
            this.textBoxElevator3Open.TabIndex = 27;
            // 
            // textBoxElevator2Open
            // 
            this.textBoxElevator2Open.Location = new System.Drawing.Point(195, 188);
            this.textBoxElevator2Open.Name = "textBoxElevator2Open";
            this.textBoxElevator2Open.Size = new System.Drawing.Size(177, 20);
            this.textBoxElevator2Open.TabIndex = 26;
            // 
            // textBoxElevator1Open
            // 
            this.textBoxElevator1Open.Location = new System.Drawing.Point(12, 188);
            this.textBoxElevator1Open.Name = "textBoxElevator1Open";
            this.textBoxElevator1Open.Size = new System.Drawing.Size(159, 20);
            this.textBoxElevator1Open.TabIndex = 25;
            // 
            // ElevatorEmulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 398);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.textBoxElevator3Open);
            this.Controls.Add(this.textBoxElevator2Open);
            this.Controls.Add(this.textBoxElevator1Open);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.buttonCallUp_13);
            this.Controls.Add(this.buttonCallDown_13);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCallUp_5);
            this.Controls.Add(this.buttonCallDown_5);
            this.Controls.Add(this.textBoxElevator3OpenReception);
            this.Controls.Add(this.textBoxElevator2OpenReception);
            this.Controls.Add(this.textBoxElevator1OpenReception);
            this.Controls.Add(this.elevator3);
            this.Controls.Add(this.elevator2);
            this.Controls.Add(this.elevator1);
            this.Name = "ElevatorEmulator";
            this.Text = "ElevatorSimulator";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCallDown_0;
        private System.Windows.Forms.TextBox elevator1;
        private System.Windows.Forms.TextBox elevator2;
        private System.Windows.Forms.TextBox elevator3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonCallUp_0;
        private System.Windows.Forms.TextBox textBoxElevator1OpenReception;
        private System.Windows.Forms.TextBox textBoxElevator2OpenReception;
        private System.Windows.Forms.TextBox textBoxElevator3OpenReception;
        private System.Windows.Forms.Button buttonCallUp_5;
        private System.Windows.Forms.Button buttonCallDown_5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonCallUp_13;
        private System.Windows.Forms.Button buttonCallDown_13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxElevator3Open;
        private System.Windows.Forms.TextBox textBoxElevator2Open;
        private System.Windows.Forms.TextBox textBoxElevator1Open;
    }
}

