namespace Debugger
{
    partial class Toolbar
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
            this.ssMainStatus = new System.Windows.Forms.StatusStrip();
            this.btnInfluenceMap = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ssMainStatus
            // 
            this.ssMainStatus.Location = new System.Drawing.Point(0, 51);
            this.ssMainStatus.Name = "ssMainStatus";
            this.ssMainStatus.Size = new System.Drawing.Size(455, 22);
            this.ssMainStatus.TabIndex = 1;
            this.ssMainStatus.Text = "statusStrip1";
            // 
            // btnInfluenceMap
            // 
            this.btnInfluenceMap.Location = new System.Drawing.Point(12, 12);
            this.btnInfluenceMap.Name = "btnInfluenceMap";
            this.btnInfluenceMap.Size = new System.Drawing.Size(97, 23);
            this.btnInfluenceMap.TabIndex = 2;
            this.btnInfluenceMap.Text = "Influence Map";
            this.btnInfluenceMap.UseVisualStyleBackColor = true;
            this.btnInfluenceMap.Click += new System.EventHandler(this.btnInfluenceMap_Click);
            // 
            // Toolbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 73);
            this.Controls.Add(this.btnInfluenceMap);
            this.Controls.Add(this.ssMainStatus);
            this.Name = "Toolbar";
            this.Text = "Starcraft ProxyBot AI Debugger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip ssMainStatus;
        private System.Windows.Forms.Button btnInfluenceMap;
    }
}

