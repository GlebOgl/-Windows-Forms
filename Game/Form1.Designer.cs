using Game.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public class WinWindow: Form
    {
        public WinWindow()
        {
            ClientSize = new Size(500, 350);
            FormBorderStyle = FormBorderStyle.FixedDialog;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(
                Brushes.Gray, 0, 0, 500, 350);
            e.Graphics.DrawString("Вам удалось спасти свое \n убежище и его жителей", new Font("Arial", 24), Brushes.Black, 0, 125);
            e.Graphics.DrawString("You Win!", new Font("Arial", 24), Brushes.Blue, 200, 300);
        }
    }

    public class FirstWindow : Form
    {
        public FirstWindow()
        {
            ClientSize = new Size(500, 350);
            FormBorderStyle = FormBorderStyle.FixedDialog;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(
                Brushes.Gray, 0, 0, 700, 350);
            e.Graphics.DrawString(" Вам нужно спасти \n убежище и его жителей \n" +
                " от захватчиков \n чтоби ходить бить \n и поднимать оружие нажимайте \n мышью на врагов и клетки \n машина для клонирования \n позволяет вам не бояться \n смерти", new Font("Arial", 24), Brushes.Black, 0, 0);
            
        }
    }
    public class GameWindow: Form
    {
        ///// <summary>
        ///// Обязательная переменная конструктора.
        ///// </summary>
        //private System.ComponentModel.IContainer components = null;

        ///// <summary>
        ///// Освободить все используемые ресурсы.
        ///// </summary>
        ///// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private readonly Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
        private readonly GameState gameState;
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private int tickCount;


        public GameWindow(DirectoryInfo imagesDirectory = null)
        {
            gameState = new GameState();
            ClientSize = new Size(
                GameState.ElementSize * MapModel.MapWidth,
                GameState.ElementSize * MapModel.MapHeight + 2 * GameState.ElementSize);
            
            //var button = new Button
            //{
            //    Location = new Point(0,
            //    GameState.ElementSize * MapModel.MapHeight),
            //    Size = new Size(GameState.ElementSize * MapModel.MapWidth, GameState.ElementSize),
            //    Text = "Пошаговый режим OFF"
            //};
            //Controls.Add(button);
            //button.Click += BattleModEnable;
            //for (var i = 0; i<MapModel.MapWidth;i++)
            //    for (var j = 0; j<MapModel.MapHeight;j++)
            //    {
            //        var button = new Button
            //        {
            //            Location = new Point(GameState.ElementSize * j, GameState.ElementSize * i),
            //            Size = new Size(GameState.ElementSize, GameState.ElementSize),
            //            Image = 
            //        };
            //        Controls.Add(button);
            //    };
            FormBorderStyle = FormBorderStyle.FixedDialog;
            if (imagesDirectory == null)
                imagesDirectory = new DirectoryInfo("Images");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
            this.MouseDown += new MouseEventHandler(MouseClickOnCell);
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += TimerTick;
            timer.Start();
        }

        //private void BattleModEnable(object sender, EventArgs e)
        //{
        //    Button b = (Button)sender;
        //    if (GameState.IsBattleModOn)
        //    {
        //        GameState.IsBattleModOn = false;
        //        GameState.IsPlayerTurn = false;
        //        b.Text = ("Пошаговый режим OFF");
        //    }
        //    else
        //    {
        //        GameState.IsBattleModOn = true;
        //        GameState.IsPlayerTurn = true;
        //        b.Text = ("Пошаговый режим ON");
        //    }
        //}

        private void MouseClickOnCell(object sender, MouseEventArgs e)
        {
            MapModel.PointClick = e.Location;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = "Game";
            DoubleBuffered = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
            MapModel.KeyPressed = e.KeyCode;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
            MapModel.KeyPressed = pressedKeys.Any() ? pressedKeys.Min() : Keys.None;
        }

        //private int ButtonClick(object sender, EventArgs e)
        //{
        //    return 1;
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            for (var i = 0; i < MapModel.MapWidth; i++)
                for (var j = 0; j < MapModel.MapHeight; j++)
                    e.Graphics.DrawImage(bitmaps["Floor.png"], new Point(i * GameState.ElementSize,
                        j * GameState.ElementSize));
            e.Graphics.DrawString("Здоровье", new Font("Arial", 16), Brushes.Black, 0,
                GameState.ElementSize * MapModel.MapHeight);
            //if (!GameState.IsPlayerAlive)
            //{
            //    var heal = 2;
            //    var endur = 3;
            //    var str = 2;
            //    var dex = 2;
            //    var paramCount = 5;
            //    var param = new Size(GameState.ElementSize * 5, GameState.ElementSize * 4);
            //    var buttonH = new Button
            //    {
            //        Location = new Point(0,0),
            //        Size = new Size(GameState.ElementSize * 2, GameState.ElementSize),
            //        Text = "MaxHealth"
            //    };
            //    var buttonE = new Button
            //    {
            //        Location = new Point(GameState.ElementSize, 0),
            //        Size = new Size(GameState.ElementSize * 2, GameState.ElementSize),
            //        Text = "Endurance"
            //    };
            //    var buttonS = new Button
            //    {
            //        Location = new Point(GameState.ElementSize * 2, 0),
            //        Size = new Size(GameState.ElementSize * 2, GameState.ElementSize),
            //        Text = "Strength"
            //    };
            //    var buttonD = new Button
            //    {
            //        Location = new Point(GameState.ElementSize * 3, 0),
            //        Size = new Size(GameState.ElementSize * 2, GameState.ElementSize),
            //        Text = "Dexterity"
            //    };
            //    buttonH.Click += (sender, args) =>
            //    {
            //        heal++;
            //        paramCount--;
            //    };
            //    buttonE.Click += (sender, args) =>
            //    {
            //        endur++;
            //        paramCount--;
            //    };
            //    buttonS.Click += (sender, args) =>
            //    {
            //        str++;
            //        paramCount--;
            //    };
            //    buttonD.Click += (sender, args) =>
            //    {
            //        dex++;
            //        paramCount--;
            //    };
            //    while (paramCount != 0)
            //    {
            //        e.Graphics.DrawString("maxHealth", new Font("Arial", 10), Brushes.Black, GameState.ElementSize * 2, 0);
            //        e.Graphics.DrawString(heal.ToString(), new Font("Arial", 10), Brushes.Black, GameState.ElementSize * 3, 0);
            //        e.Graphics.DrawString("Endurance", new Font("Arial", 10), Brushes.Black, GameState.ElementSize * 2, GameState.ElementSize);
            //        e.Graphics.DrawString(endur.ToString(), new Font("Arial", 10), Brushes.Black, GameState.ElementSize * 3, GameState.ElementSize);
            //        e.Graphics.DrawString("Strength", new Font("Arial", 10), Brushes.Black, GameState.ElementSize * 2, GameState.ElementSize * 2);  
            //        e.Graphics.DrawString(heal.ToString(), new Font("Arial", 10), Brushes.Black, GameState.ElementSize * 3, GameState.ElementSize * 2);
            //        e.Graphics.DrawString("Dexterity", new Font("Arial", 10), Brushes.Black, GameState.ElementSize * 2, GameState.ElementSize * 3);
            //        e.Graphics.DrawString(heal.ToString(), new Font("Arial", 10), Brushes.Black, GameState.ElementSize * 3, GameState.ElementSize * 3);
            //    }
            //    MapCreator.Dexterity = dex;
            //    MapCreator.Endurance = endur;
            //    MapCreator.MaxHealth = heal;
            //    MapCreator.Strength = str;
            //}
            var health = GameState.PlayerHealth.ToString();
            var endur = GameState.PlayerEndurance.ToString();
            var range = GameState.PlayerRange.ToString();
            var dmg = GameState.PlayerDamage.ToString();
            e.Graphics.DrawString(health, new Font("Arial", 16), Brushes.Black, 150,
                GameState.ElementSize * MapModel.MapHeight);
            e.Graphics.DrawString("Выносливость", new Font("Arial", 16), Brushes.Black, 0,
                GameState.ElementSize * MapModel.MapHeight + GameState.ElementSize);
            e.Graphics.DrawString(endur, new Font("Arial", 16), Brushes.Black, 150,
                GameState.ElementSize * MapModel.MapHeight + GameState.ElementSize);
            e.Graphics.DrawString("Дальность атаки", new Font("Arial", 16), Brushes.Black, 300,
                GameState.ElementSize * MapModel.MapHeight);
            e.Graphics.DrawString(range, new Font("Arial", 16), Brushes.Black, 500,
                GameState.ElementSize * MapModel.MapHeight);
            e.Graphics.DrawString("Урон", new Font("Arial", 16), Brushes.Black, 300,
                GameState.ElementSize * MapModel.MapHeight + GameState.ElementSize);
            e.Graphics.DrawString(dmg, new Font("Arial", 16), Brushes.Black, 500,
                GameState.ElementSize * MapModel.MapHeight + GameState.ElementSize);
            //e.Graphics.FillRectangle(
            //    Brushes.Gray, 0, 0, GameState.ElementSize * MapModel.MapWidth,
            //    GameState.ElementSize * MapModel.MapHeight);
            
            foreach (var a in gameState.Animations)
            {
                if (a.Structure.GetImageFileName() == "Empty")
                    continue;
                e.Graphics.DrawImage(bitmaps[a.Structure.GetImageFileName()], a.Location);
            }
            
            e.Graphics.ResetTransform();
            
            //e.Graphics.DrawString("sdas", new Font("Arial", 16), Brushes.Green, GameState.ElementSize * MapModel.MapWidth - GameState.ElementSize,
            //    GameState.ElementSize * MapModel.MapHeight);
        }

        private void TimerTick(object sender, EventArgs args)
        {
            if (tickCount == 0) gameState.BeginAct();
            foreach (var e in gameState.Animations)
                e.Location = new Point(e.Location.X + 4 * e.Command.DeltaX, e.Location.Y + 4 * e.Command.DeltaY);
            if (tickCount == 7)
                gameState.EndAct();
            tickCount++;
            
            
            if (tickCount == 8) tickCount = 0;
            Invalidate();
        }


//#region Код, автоматически созданный конструктором форм Windows

///// <summary>
///// Требуемый метод для поддержки конструктора — не изменяйте 
///// содержимое этого метода с помощью редактора кода.
///// </summary>
//private void InitializeComponent()
//        {
//            this.components = new System.ComponentModel.Container();
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(800, 450);
//            this.Text = "Form1";
//        }

//        #endregion
    }
}

