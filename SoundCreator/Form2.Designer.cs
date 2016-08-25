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
			this.pbRecWave = new System.Windows.Forms.PictureBox();
			this.pbRecLevel = new System.Windows.Forms.PictureBox();
			this.PlayRec = new System.Windows.Forms.Button();
			this.CancelRec = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pbRecWave)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRecLevel)).BeginInit();
			this.SuspendLayout();
			// 
			// StartRec
			// 
			this.StartRec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.StartRec.Location = new System.Drawing.Point(12, 438);
			this.StartRec.Name = "StartRec";
			this.StartRec.Size = new System.Drawing.Size(75, 23);
			this.StartRec.TabIndex = 0;
			this.StartRec.Text = "Start";
			this.StartRec.UseVisualStyleBackColor = true;
			this.StartRec.Click += new System.EventHandler(this.StartRec_Click);
			// 
			// StopRec
			// 
			this.StopRec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.StopRec.Location = new System.Drawing.Point(93, 438);
			this.StopRec.Name = "StopRec";
			this.StopRec.Size = new System.Drawing.Size(75, 23);
			this.StopRec.TabIndex = 1;
			this.StopRec.Text = "Stop";
			this.StopRec.UseVisualStyleBackColor = true;
			this.StopRec.Click += new System.EventHandler(this.StopRec_Click);
			// 
			// ExitRec
			// 
			this.ExitRec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ExitRec.Location = new System.Drawing.Point(921, 438);
			this.ExitRec.Name = "ExitRec";
			this.ExitRec.Size = new System.Drawing.Size(75, 23);
			this.ExitRec.TabIndex = 2;
			this.ExitRec.Text = "Ok";
			this.ExitRec.UseVisualStyleBackColor = true;
			this.ExitRec.Click += new System.EventHandler(this.ExitRec_Click);
			// 
			// pbRecWave
			// 
			this.pbRecWave.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pbRecWave.BackColor = System.Drawing.Color.Black;
			this.pbRecWave.Location = new System.Drawing.Point(45, 12);
			this.pbRecWave.Name = "pbRecWave";
			this.pbRecWave.Size = new System.Drawing.Size(951, 420);
			this.pbRecWave.TabIndex = 3;
			this.pbRecWave.TabStop = false;
			this.pbRecWave.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbRecWave_MouseDown);
			this.pbRecWave.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbRecWave_MouseMove);
			this.pbRecWave.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbRecWave_MouseUp);
			// 
			// pbRecLevel
			// 
			this.pbRecLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.pbRecLevel.BackColor = System.Drawing.Color.Black;
			this.pbRecLevel.Location = new System.Drawing.Point(11, 12);
			this.pbRecLevel.Name = "pbRecLevel";
			this.pbRecLevel.Size = new System.Drawing.Size(28, 420);
			this.pbRecLevel.TabIndex = 4;
			this.pbRecLevel.TabStop = false;
			// 
			// PlayRec
			// 
			this.PlayRec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.PlayRec.Location = new System.Drawing.Point(174, 438);
			this.PlayRec.Name = "PlayRec";
			this.PlayRec.Size = new System.Drawing.Size(75, 23);
			this.PlayRec.TabIndex = 6;
			this.PlayRec.Text = "Play";
			this.PlayRec.UseVisualStyleBackColor = true;
			this.PlayRec.Click += new System.EventHandler(this.PlayRec_Click);
			// 
			// CancelRec
			// 
			this.CancelRec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelRec.Location = new System.Drawing.Point(840, 438);
			this.CancelRec.Name = "CancelRec";
			this.CancelRec.Size = new System.Drawing.Size(75, 23);
			this.CancelRec.TabIndex = 7;
			this.CancelRec.Text = "Cancel";
			this.CancelRec.UseVisualStyleBackColor = true;
			this.CancelRec.Click += new System.EventHandler(this.CancelRec_Click);
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 473);
			this.Controls.Add(this.CancelRec);
			this.Controls.Add(this.PlayRec);
			this.Controls.Add(this.pbRecLevel);
			this.Controls.Add(this.pbRecWave);
			this.Controls.Add(this.ExitRec);
			this.Controls.Add(this.StopRec);
			this.Controls.Add(this.StartRec);
			this.Name = "Form2";
			this.Text = "Record";
			((System.ComponentModel.ISupportInitialize)(this.pbRecWave)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRecLevel)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button StopRec;
		private System.Windows.Forms.Button ExitRec;
		private System.Windows.Forms.PictureBox pbRecWave;
		private System.Windows.Forms.PictureBox pbRecLevel;
		public System.Windows.Forms.Button StartRec;
		private System.Windows.Forms.Button PlayRec;
		private System.Windows.Forms.Button CancelRec;
	}
}