namespace SoundCreator {
	partial class Form2 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing ) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.StartRec = new System.Windows.Forms.Button();
			this.StopRec = new System.Windows.Forms.Button();
			this.ExitRec = new System.Windows.Forms.Button();
			this.pbRecTime = new System.Windows.Forms.PictureBox();
			this.pbRecLevel = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.Buffer1 = new System.Windows.Forms.RadioButton();
			this.Buffer2 = new System.Windows.Forms.RadioButton();
			this.Buffer3 = new System.Windows.Forms.RadioButton();
			this.Buffer4 = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.pbRecTime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRecLevel)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// StartRec
			// 
			this.StartRec.Location = new System.Drawing.Point(11, 110);
			this.StartRec.Name = "StartRec";
			this.StartRec.Size = new System.Drawing.Size(75, 23);
			this.StartRec.TabIndex = 0;
			this.StartRec.Text = "Start";
			this.StartRec.UseVisualStyleBackColor = true;
			// 
			// StopRec
			// 
			this.StopRec.Location = new System.Drawing.Point(92, 110);
			this.StopRec.Name = "StopRec";
			this.StopRec.Size = new System.Drawing.Size(75, 23);
			this.StopRec.TabIndex = 1;
			this.StopRec.Text = "Stop";
			this.StopRec.UseVisualStyleBackColor = true;
			// 
			// ExitRec
			// 
			this.ExitRec.Location = new System.Drawing.Point(173, 110);
			this.ExitRec.Name = "ExitRec";
			this.ExitRec.Size = new System.Drawing.Size(75, 23);
			this.ExitRec.TabIndex = 2;
			this.ExitRec.Text = "Exit";
			this.ExitRec.UseVisualStyleBackColor = true;
			this.ExitRec.Click += new System.EventHandler(this.ExitRec_Click);
			// 
			// pbRecTime
			// 
			this.pbRecTime.BackColor = System.Drawing.Color.Black;
			this.pbRecTime.Location = new System.Drawing.Point(11, 94);
			this.pbRecTime.Name = "pbRecTime";
			this.pbRecTime.Size = new System.Drawing.Size(237, 10);
			this.pbRecTime.TabIndex = 3;
			this.pbRecTime.TabStop = false;
			// 
			// pbRecLevel
			// 
			this.pbRecLevel.BackColor = System.Drawing.Color.Black;
			this.pbRecLevel.Location = new System.Drawing.Point(11, 78);
			this.pbRecLevel.Name = "pbRecLevel";
			this.pbRecLevel.Size = new System.Drawing.Size(237, 10);
			this.pbRecLevel.TabIndex = 4;
			this.pbRecLevel.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.Buffer4);
			this.groupBox1.Controls.Add(this.Buffer3);
			this.groupBox1.Controls.Add(this.Buffer2);
			this.groupBox1.Controls.Add(this.Buffer1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(236, 60);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sound";
			// 
			// Buffer1
			// 
			this.Buffer1.AutoSize = true;
			this.Buffer1.Location = new System.Drawing.Point(6, 19);
			this.Buffer1.Name = "Buffer1";
			this.Buffer1.Size = new System.Drawing.Size(62, 17);
			this.Buffer1.TabIndex = 0;
			this.Buffer1.TabStop = true;
			this.Buffer1.Text = "Buffer 1";
			this.Buffer1.UseVisualStyleBackColor = true;
			// 
			// Buffer2
			// 
			this.Buffer2.AutoSize = true;
			this.Buffer2.Location = new System.Drawing.Point(6, 37);
			this.Buffer2.Name = "Buffer2";
			this.Buffer2.Size = new System.Drawing.Size(62, 17);
			this.Buffer2.TabIndex = 1;
			this.Buffer2.TabStop = true;
			this.Buffer2.Text = "Buffer 2";
			this.Buffer2.UseVisualStyleBackColor = true;
			// 
			// Buffer3
			// 
			this.Buffer3.AutoSize = true;
			this.Buffer3.Location = new System.Drawing.Point(145, 19);
			this.Buffer3.Name = "Buffer3";
			this.Buffer3.Size = new System.Drawing.Size(62, 17);
			this.Buffer3.TabIndex = 2;
			this.Buffer3.TabStop = true;
			this.Buffer3.Text = "Buffer 3";
			this.Buffer3.UseVisualStyleBackColor = true;
			// 
			// Buffer4
			// 
			this.Buffer4.AutoSize = true;
			this.Buffer4.Location = new System.Drawing.Point(145, 37);
			this.Buffer4.Name = "Buffer4";
			this.Buffer4.Size = new System.Drawing.Size(62, 17);
			this.Buffer4.TabIndex = 3;
			this.Buffer4.TabStop = true;
			this.Buffer4.Text = "Buffer 4";
			this.Buffer4.UseVisualStyleBackColor = true;
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(260, 141);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pbRecLevel);
			this.Controls.Add(this.pbRecTime);
			this.Controls.Add(this.ExitRec);
			this.Controls.Add(this.StopRec);
			this.Controls.Add(this.StartRec);
			this.Name = "Form2";
			this.Text = "Record";
			((System.ComponentModel.ISupportInitialize)(this.pbRecTime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRecLevel)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button StartRec;
		private System.Windows.Forms.Button StopRec;
		private System.Windows.Forms.Button ExitRec;
		private System.Windows.Forms.PictureBox pbRecTime;
		private System.Windows.Forms.PictureBox pbRecLevel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton Buffer4;
		private System.Windows.Forms.RadioButton Buffer3;
		private System.Windows.Forms.RadioButton Buffer2;
		private System.Windows.Forms.RadioButton Buffer1;
	}
}