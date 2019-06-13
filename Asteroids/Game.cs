﻿using System.Windows.Forms;
using System.Drawing;
using System;
using Asteroids.Objects;
using System.Media;
using System.Windows.Media;
using Color = System.Drawing.Color;
using Brushes = System.Drawing.Brushes;

namespace Asteroids
{
    class Game
    {
        /// <summary>
        /// Плеййер для снаряда
        /// </summary>
        public static MediaPlayer bul = new MediaPlayer();
        public static string pathToFileBul = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shot.wav");

        /// <summary>
        /// Плейер для астероида
        /// </summary>
        public static MediaPlayer aster = new MediaPlayer();
        public static string pathToFileAster = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "band.wav");


        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;

        protected static Star[] _stars;
        protected static Background _background;
        protected static Asteroid[] _asteroids;
        protected static Planets[] _planets;
        private static Bullet _bullet;
        private static Ship _ship;

        private static int width;
        private static int height;

        public static int GetWidth() => width;
        public static int GetHeight() => height;

        public static void SetWidth(int value)
        {
            //if (value >= GetWidth() || value < 0) throw new ArgumentOutOfRangeException("Выход за пределы игрового поля");
            width = value;
        }
        public static void SetHeight(int value)
        {
            //if (value >= GetHeight() || value < 0) throw new ArgumentOutOfRangeException("Выход за пределы игрового поля");
            height = value;
        }

        static Game()
        {
        }

        /// <summary>
        /// Инициализация формы
        /// </summary>
        /// <param name="form"></param>
        public static void Init(Form form)
        {
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Text = "Asteroids";
            SetWidth(form.Width);
            SetHeight(form.Height);
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.MaximumSize = new Size(GetWidth(), GetHeight());
            form.MinimumSize = new Size(GetWidth(), GetHeight());
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.WindowState = FormWindowState.Normal;
            form.WindowState = FormWindowState.Maximized;//полный экран
            form.FormBorderStyle = FormBorderStyle.None;//полный экран


            Graphics g;
            g = form.CreateGraphics();
            _context = BufferedGraphicsManager.Current;
            Buffer = _context.Allocate(g, new Rectangle(0, 0, GetWidth(), GetHeight()));

            Load();

            Timer timer = new Timer { Interval = 40 };
            timer.Tick += Timer_Tick;
            timer.Start();

            form.KeyDown += Form_KeyDown;
        }


        /// <summary>
        /// Метод загрузки в память создаваемых объектов
        /// </summary>
        public static void Load()
        {
            int s, p;
            Random rnd = new Random();            
            _stars = new Star[100];
            _background = new Background(new Point(0, 0), new Point(0, 0), new Size(0,0));
            _asteroids = new Asteroid[6];
            _planets = new Planets[1];
            _bullet = new Bullet(new Point(0, rnd.Next(0, Game.height)), new Point(0,0), new Size(30,1));
            _ship = new Ship(new Point(10, 400), new Point(5, 5), new Size(50, 50));

            for (int i = 0; i < _asteroids.Length; i++)
            {
                s = rnd.Next(30, 90);
                p = rnd.Next(50, 980);
                _asteroids[i] = new Asteroid(new Point(1925, p), new Point(15, 0), new Size(s, s), rnd.Next(4));
            }

            for (int i = 0; i < _stars.Length; i++)
            {
                s = rnd.Next(1, 2);
                p = rnd.Next(0, 1080);
                _stars[i] = new Star(new Point(1925, rnd.Next(0,Game.height)), new Point(s - i, s), new Size(s, s));
            }

            for (int i = 0; i < _planets.Length; i++)
            {
                s = rnd.Next(80, 150);
                p = rnd.Next(100, 800);
                _planets[i] = new Planets(new Point(1925, p), new Point(5, 0), new Size(s, s),rnd.Next(1));
            }
        }

        /// <summary>
        /// Метод отрисовки объектов
        /// </summary>
        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            _background.Draw();
            foreach (Star s in _stars)
                s.Draw();
            foreach (Asteroid a in _asteroids)
                a.Draw();
            foreach (Planets p in _planets)
                p.Draw();
            _bullet.Draw();
            _ship.Draw();
            if (_ship != null)
                Buffer.Graphics.DrawString("Energy:" + _ship.Energy, SystemFonts.CaptionFont, Brushes.White, 0, 0);
            Buffer.Render();
        }

        /// <summary>
        /// Метод обновления отрисованных объектов из памяти
        /// </summary>
        public static void Update()
        {
            _background.Update();
            foreach (Star s in _stars)
                s.Update();
            foreach (Asteroid a in _asteroids)
            {
                a.Update();
                if (a.Collision(_bullet))
                {
                    a.Play();
                    a.Init();
                    _bullet.Init();
                }
            }
            foreach (Planets p in _planets)
                p.Update();
            _ship.Update();
            _bullet.Update();
        }

        /// <summary>
        /// Метод задающий временной интервал во время отрисовки объектов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Timer_Tick(object sender, EventArgs e)
        {            
            Draw();
            Update();
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _bullet = new Bullet(new Point(_ship.Rect.X + 10, _ship.Rect.Y + 4), new Point(4, 0), new Size(4, 1));
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
        }
    }
}
