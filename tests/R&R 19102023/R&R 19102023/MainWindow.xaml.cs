using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
using System.Windows.Controls.Primitives;

namespace R_R_19102023
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.player = new Rectangle();
            CompositionTarget.Rendering += loop;
            this.setPlayer();
            this.spawnApple(null);
            this.spawnTree(rand.Next(200, 700), rand.Next(200, 350));
            this.spawnSpeedup();
            //this.spawnPuddle();
            //this.spawnSlowdown();
            this.start();
            this.window.MouseWheel += mouseWheelMoved;
            this.versionstr.Content = $"AppleCatcher {ver}";
        }

        private void mouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            if (uiWelcome.Visibility == Visibility.Visible)
            {
                int i1 = e.Delta;
                if (i1 < 0)
                {
                    foreach (Canvas listing in profiles.Children)
                    {
                        Canvas.SetTop(listing, Canvas.GetTop(listing) - 5);
                    }
                }
                if (i1 > 0)
                {
                    foreach (Canvas listing in profiles.Children)
                    {
                        Canvas.SetTop(listing, Canvas.GetTop(listing) + 5);
                    }
                }
            }
        }

        Rectangle player;
        long score = 0;
        long time = 60;
        long timePlayed = 0;
        long fps = 0;
        long exp = 0;
        long expr = 100;
        long level = 1;
        public double playerX = 0;
        public double playerY = 0;
        public double appleX = 0;
        public double appleY = 0;
        public double puddleGradientStop = 0.0;
        double speed = 2;
        public string movementdir = "";
        Random rand = new Random();
        public Stopwatch timer = new Stopwatch();
        public Stopwatch frameratetimer = new Stopwatch();
        public Stopwatch updatetimer = new Stopwatch();
        public bool timerShouldTick = true;
        public bool endScreenIsShown = false;
        public bool isOnTitleScreen = true;
        public bool isShowingNotif = false;
        public string gameDir = Directory.GetCurrentDirectory();
        public string ver = "1.0";
        public string selectedprofile = null;
        public string playername = "Player 1";
        public int totalscore = 0;
        public int besttime = 0;
        public int effecttimer = 0;
        public int treespawnertimer = 0;
        public int autosaveTimer = 0;
        public int notifTimer = 0;
        public int treesCount = 0;
        public string createProfileName = "null";
        public string profileColorCode = "null";
        public object[] treesInGame;
        public Label playerNameLabel = new Label();

        private void loop(object sender, EventArgs e)
        {
            if (effecttimer < 3)
            {
                ++effecttimer;
            }
            else
            {
                effecttimer = 0;
                if (this.puddleGradientStop < 2.0)
                {
                    this.puddleGradientStop += 0.1;
                }
                else
                {
                    this.puddleGradientStop = -2.0;
                }
                foreach(Canvas puddle in this.puddles.Children)
                {
                    foreach (object rect in puddle.Children)
                    {
                        Rectangle rect1 = new Rectangle();
                        rect1.Name = "genericrect";
                        if (rect is Rectangle)
                        {
                            rect1 = (Rectangle)rect;
                        }
                        if (rect1.Name != "puddledecor")
                        {
                            LinearGradientBrush brush = new LinearGradientBrush();
                            brush.StartPoint = new Point(0.0, 0.0);
                            brush.EndPoint = new Point(1.0, 0.0);
                            GradientStop stop1 = new GradientStop(Colors.Blue, this.puddleGradientStop - 0.75);
                            GradientStop stop2 = new GradientStop(Colors.Cyan, this.puddleGradientStop);
                            GradientStop stop3 = new GradientStop(Colors.Blue, this.puddleGradientStop + 0.75);
                            brush.GradientStops.Add(stop1);
                            brush.GradientStops.Add(stop2);
                            brush.GradientStops.Add(stop3);
                            rect1.Fill = brush;
                        }
                    }
                }
                object[] effects = new object[3];
                for (int i1 = 0; i1 < 3; ++i1)
                {
                    Ellipse effect = new Ellipse();
                    effect.Width = 5;
                    effect.Height = 5;
                    effect.Fill = this.player.Fill;
                    effects[i1] = effect;
                }
                foreach (Ellipse effect in effects)
                {
                    backgrounddecors.Children.Add(effect);
                    double posX = rand.Next((int)this.playerX + 5, (int)this.playerX + (int)this.player.Width - 5);
                    double posY = rand.Next((int)this.playerY + 5, (int)this.playerY + (int)this.player.Height - 5);
                    Canvas.SetLeft(effect, posX);
                    Canvas.SetTop(effect, posY);
                    DoubleAnimation anim = new DoubleAnimation();
                    anim.From = 1;
                    anim.To = 0;
                    anim.Duration = new Duration(TimeSpan.FromSeconds(5));
                    anim.Completed += (s, EventArgs) => removeEffect(effect);
                    effect.BeginAnimation(OpacityProperty, anim);
                }
            }
            if (treespawnertimer < 100)
            {
                ++treespawnertimer;
            }
            else
            {
                //this.generateTrees();
                this.treespawnertimer = 0;
            }
            ++this.fps;
            endScreenBackground.Width = this.window.Width;
            endScreenBackground.Height = this.window.Height;
            Canvas.SetLeft(screenText, this.window.Width / 2 - screenText.Width / 2);
            Canvas.SetTop(screenText, this.window.Height / 2 - screenText.Height / 2);
            Canvas.SetLeft(endScreenText, this.window.Width / 2 - endScreenText.Width / 2);
            Canvas.SetTop(endScreenText, this.window.Height / 2 - endScreenText.Height / 2);
            Canvas.SetLeft(welcomeScreenText, this.window.Width / 2 - endScreenText.Width / 2 - 280);
            Canvas.SetTop(welcomeScreenText, this.window.Height / 2 - endScreenText.Height / 2 - 180);
            Canvas.SetLeft(keybindRetryText, this.window.Width / 2 - keybindRetryText.Width / 2);
            Canvas.SetTop(keybindRetryText, this.window.Height / 2 - keybindRetryText.Height / 2 + 70);
            Canvas.SetLeft(keybindQuitText, this.window.Width / 2 - keybindQuitText.Width / 2);
            Canvas.SetTop(keybindQuitText, this.window.Height / 2 - keybindQuitText.Height / 2 + 100);
            Canvas.SetLeft(playtimeText, this.window.Width / 2 - playtimeText.Width / 2);
            Canvas.SetTop(playtimeText, this.window.Height / 2 - playtimeText.Height / 2 + 40);
            Canvas.SetLeft(this.playerNameLabel, this.playerX - this.playerNameLabel.Width / 2 + this.player.Width / 2);
            Canvas.SetTop(this.playerNameLabel, this.playerY - 50);
            Canvas.SetTop(this.fpsText, this.window.Height - 90);
            this.camera.ScrollToHorizontalOffset(this.playerX - this.Width / 2 + this.player.Width / 2);
            this.camera.ScrollToVerticalOffset(this.playerY - this.Height / 2 + this.player.Height / 2);
            this.camera.Width = this.window.Width;
            this.camera.Height = this.window.Height;
            if (this.isOnTitleScreen)
            {
            }
            if (!this.frameratetimer.IsRunning)
            {
                this.frameratetimer.Start();
            }
            if (this.frameratetimer.ElapsedMilliseconds >= 1000)
            {
                if (this.isShowingNotif)
                {
                    ++this.notifTimer;
                }
                if (this.isShowingNotif && this.notifTimer >= 3)
                {
                    this.hideNotification();
                }
                ++this.autosaveTimer;
                if (this.autosaveTimer == 60)
                {
                    if (this.selectedprofile != null)
                    {
                        this.writeProfile(this.selectedprofile);
                        this.showNotification("Progress saved!");
                    }
                    else
                    {
                        this.showNotification("Not loaded");
                    }
                    this.autosaveTimer = 0;
                }
                this.fpsText.Content = $"Versie {ver}\n{this.fps} FPS";
                this.fps = 0;
                this.frameratetimer.Restart();
            }
            if (!this.isOnTitleScreen)
            {
                if (!this.timer.IsRunning && this.timerShouldTick)
                {
                    this.timer.Start();
                }
                if (this.timer.IsRunning && !this.timerShouldTick)
                {
                    this.timer.Stop();
                }
                if (this.timer.ElapsedMilliseconds >= 1000)
                {
                    --time;
                    ++timePlayed;
                    this.timer.Restart();
                    if (this.time < 11)
                    {
                        showScreenText(1, 1);
                        this.screenText.Foreground = Brushes.Red;
                        this.screenText.Content = $"{time}";
                    }
                }
                if (time <= 0)
                {
                    if (timerShouldTick)
                    {
                        showEndScreen();
                        this.timerShouldTick = false;
                        this.screenText.Foreground = Brushes.Red;
                        this.screenText.Content = $"Time's up!";
                        this.speed = 0;
                    }
                }
                if (time >= 11)
                {
                    screenText.Content = null;
                    endScreenText.Visibility = Visibility.Hidden;
                }
                Canvas.SetLeft(this.player, this.playerX);
                Canvas.SetTop(this.player, this.playerY);
                if (endScreenIsShown)
                {
                    if (Keyboard.IsKeyDown(Key.R))
                    {
                        endScreenIsShown = false;
                        this.time = 60;
                        this.timerShouldTick = true;
                        this.score = 0;
                        endScreen.Visibility = Visibility.Hidden;
                        this.speed = 2;
                        DoubleAnimation anim = new DoubleAnimation();
                        anim.From = 1;
                        anim.To = 0;
                        anim.Duration = new Duration(TimeSpan.FromSeconds(0.01));
                        playtimeText.BeginAnimation(OpacityProperty, anim);
                        keybindRetryText.BeginAnimation(OpacityProperty, anim);
                        keybindQuitText.BeginAnimation(OpacityProperty, anim);
                    }
                    if (Keyboard.IsKeyDown(Key.Q))
                    {
                        Environment.Exit(0);
                    }
                }
                if (Keyboard.IsKeyDown(Key.W))
                {
                    this.movementdir = "y-";
                }
                if (Keyboard.IsKeyDown(Key.S))
                {
                    this.movementdir = "y+";
                }
                if (Keyboard.IsKeyDown(Key.A))
                {
                    this.movementdir = "x-";
                }
                if (Keyboard.IsKeyDown(Key.D))
                {
                    this.movementdir = "x+";
                }
                if (Keyboard.IsKeyDown(Key.D0))
                {
                    this.showNotification("test");
                }
                if (this.movementdir == "x-")
                {
                    this.playerX -= speed;
                }
                if (this.movementdir == "x+")
                {
                    this.playerX += speed;
                }
                if (this.movementdir == "y-")
                {
                    this.playerY -= speed;
                }
                if (this.movementdir == "y+")
                {
                    this.playerY += speed;
                }
                if (Keyboard.IsKeyDown(Key.F5))
                {
                    this.spawnApple(null);
                }
                if (Keyboard.IsKeyDown(Key.F6))
                {
                    this.spawnSpeedup();
                }
                if (Keyboard.IsKeyDown(Key.F7))
                {
                    this.spawnSlowdown();
                }
                if (Keyboard.IsKeyDown(Key.F8))
                {
                    this.spawnTree(rand.Next(200, 700), rand.Next(200, 350));
                }
                if (Keyboard.IsKeyDown(Key.F10))
                {
                    this.playerX = 0;
                    this.playerY = 0;
                }
                if (Keyboard.IsKeyDown(Key.F11))
                {
                    this.playerX = 134217728;
                    this.playerY = 0;
                }
                foreach (Canvas apple in this.apples.Children)
                {
                    double x = Canvas.GetLeft(apple);
                    double y = Canvas.GetTop(apple);
                    if (x >= this.playerX && x <= this.playerX + 25 && y >= this.playerY && y <= this.playerY + 25 || this.playerX >= x && this.playerX <= x + 25 && this.playerY >= y && this.playerY <= y + 25)
                    {
                        ++this.score;
                        time += 3;
                        this.apples.Children.Remove(apple);
                        this.checkExp(3 * (long)speed);
                        this.spawnApple(null);
                        break;
                        //this.playSound("pickup");
                    }
                }
                foreach (Rectangle block in this.blocks.Children)
                {
                    string name = block.Name;
                    string[] namesplit = name.Split('0');
                    if (namesplit[0] == "speedup")
                    {
                        int speed = Convert.ToInt32(namesplit[1]);
                        double x = Canvas.GetLeft(block);
                        double y = Canvas.GetTop(block);
                        if (x >= this.playerX && x <= this.playerX + 25 && y >= this.playerY && y <= this.playerY + 25)
                        {
                            this.speed += speed;
                            this.blocks.Children.Remove(block);
                            this.spawnSpeedup();
                            break;
                        }
                    }
                    else if (namesplit[0] == "slowdown")
                    {
                        int speed = Convert.ToInt32(namesplit[1]);
                        double x = Canvas.GetLeft(block);
                        double y = Canvas.GetTop(block);
                        if (x >= this.playerX && x <= this.playerX + 25 && y >= this.playerY && y <= this.playerY + 25)
                        {
                            this.speed -= speed;
                            if (this.speed <= 0)
                            {
                                this.speed = 1;
                            }
                            this.blocks.Children.Remove(block);
                            this.spawnSlowdown();
                            break;
                        }
                    }
                }
                foreach (Canvas puddle in this.puddles.Children)
                {
                    double x = Canvas.GetLeft(puddle);
                    double y = Canvas.GetTop(puddle);
                    if (this.playerX >= x && this.playerX <= x + puddle.Width && this.playerY >= y && this.playerY <= y + puddle.Height)
                    {
                        if (timerShouldTick)
                        {
                            showEndScreen();
                            this.timerShouldTick = false;
                            this.screenText.Foreground = Brushes.Red;
                            this.screenText.Content = $"plons";
                            this.endScreenText.Content = $"plons";
                            this.speed = 0;
                        }
                        break;
                    }
                }
                foreach (Canvas apple in this.apples.Children)
                {
                    foreach (UIElement decor in apple.Children)
                    {
                        int type = 0;
                        Rectangle rect = null;
                        if (decor is Rectangle)
                        {
                            rect = (Rectangle)decor;
                            string decorname = rect.Name;
                            if (decorname == "A")
                            {
                                type = 1;
                            }
                            else if (decorname == "B")
                            {
                                type = 2;
                            }
                            if (type == 1)
                            {
                                Canvas.SetLeft(decor, this.appleX + apple.Width / 2 - 1.5);
                                Canvas.SetTop(decor, this.appleY - 9.5);
                            }
                            if (type == 2)
                            {
                                Canvas.SetLeft(decor, this.appleX + apple.Width / 2);
                                Canvas.SetTop(decor, this.appleY - 7.0);
                            }
                        }
                    }
                }
            }
            scoreText.Content = this.score;
            speedText.Content = this.speed;
            timeText.Content = this.time;
            pfNameText.Content = this.selectedprofile;
        }

        private void removeEffect(UIElement effect)
        {
            this.background.Children.Remove(effect);
        }

        void setPlayer()
        {
            this.player.Fill = Brushes.Black;
            this.player.Height = 25;
            this.player.Width = 25;
            this.main.Children.Add(this.player);
            Canvas.SetLeft(this.player, this.window.Width / 2 - 12.5);
            Canvas.SetTop(this.player, this.window.Height / 2 - 12.5);
            this.playerX = Canvas.GetLeft(this.player);
            this.playerY = Canvas.GetTop(this.player);
        }

        void playSound(string type)
        {
            MediaElement sound = new MediaElement();
            Uri path = null;
            if (type == "pickup")
            {
                path = new Uri($@"{this.gameDir}\media\pickup.mp3");
                sound.Source = path;
            }
            this.media.Children.Add(sound);
            sound.LoadedBehavior = MediaState.Manual;
            sound.Play();
        }

        void spawnApple(Canvas tree)
        {
            long treeX = 0;
            long treeY = 0;
            Canvas apple = new Canvas();
            Ellipse appleObject = new Ellipse();
            appleObject.Fill = Brushes.Red;
            appleObject.Height = 20;
            appleObject.Width = 20;
            apple.Height = 20;
            apple.Width = 20;
            apple.Children.Add(appleObject);
            if (tree != null)
            {
                int i = rand.Next(0, 1);
                if (i == 0)
                {
                    treeX = (long)Canvas.GetLeft(tree) + rand.Next(70, 100);
                    treeY = (long)Canvas.GetTop(tree) + rand.Next(70, 100);
                }
                else if (i == 1)
                {
                    treeX = (long)Canvas.GetLeft(tree) - rand.Next(70, 100);
                    treeY = (long)Canvas.GetTop(tree) - rand.Next(70, 100);
                }
                Canvas.SetLeft(apple, treeX);
                Canvas.SetTop(apple, treeY);
            }
            else
            {
                Canvas.SetLeft(apple, rand.Next((int)this.playerX - 200, (int)this.playerX + 200));
                Canvas.SetTop(apple, rand.Next((int)this.playerY - 200, (int)this.playerY + 200));
            }
            Rectangle decor = new Rectangle();
            decor.Height = 10;
            decor.Width = 3;
            decor.Fill = Brushes.Brown;
            decor.Name = "A";
            apple.Children.Add(decor);
            Rectangle decor2 = new Rectangle();
            decor2.Height = 3;
            decor2.Width = 7;
            decor2.Fill = Brushes.DarkGreen;
            decor2.Name = "B";
            apple.Children.Add(decor2);
            this.apples.Children.Add(apple);
        }

        void spawnTree(long x, long y)
        {
            Canvas tree = new Canvas();
            Rectangle main = new Rectangle();
            Image img = new Image();
            Ellipse leaf = new Ellipse();
            ImageSourceConverter conv = new ImageSourceConverter();
            BrushConverter conv1 = new BrushConverter();
            conv.ConvertFromString($@"{this.gameDir}\png\treebase.png");
            img.SetValue(Image.SourceProperty, conv.ConvertFromString($@"{this.gameDir}\png\treebase.png"));
            double height = 153.6;
            double width = 57.6;
            main.Fill = Brushes.Brown;
            main.Height = height;
            main.Width = width;
            leaf.Fill = Brushes.DarkGreen;
            leaf.Height = 60;
            leaf.Width = 100;
            tree.Height = height;
            tree.Width = width;
            img.Height = height;
            img.Width = width;
            tree.Children.Add(main);
            tree.Children.Add(img);
            tree.Children.Add(leaf);
            for (int i1 = 0; i1 < 30; ++i1)
            {
                Rectangle rect = new Rectangle();
                rect.Width = 5;
                rect.Height = 5;
                rect.Fill = (Brush)conv1.ConvertFromString("#FF003100");
                tree.Children.Add(rect);
                Canvas.SetLeft(rect, rand.Next(-10, 60));
                Canvas.SetTop(rect, rand.Next(-5, 30));
            }
            Canvas.SetLeft(tree, x);
            Canvas.SetTop(tree, y);
            Canvas.SetLeft(leaf, -21);
            Canvas.SetTop(leaf, -15);
            Button button = new Button();
            button.Width = tree.Width;
            button.Height = tree.Height;
            button.Click += (sender, EventArgs) => treeclick(tree);
            button.Opacity = 0;
            button.FontWeight = FontWeights.Bold;
            button.Content = "TREE";
            tree.Children.Add(button);
            Rectangle decor = new Rectangle();
            decor.Height = 10;
            decor.Width = 3;
            decor.Fill = Brushes.Brown;
            decor.Name = "A";
            //tree.Children.Add(decor);
            Rectangle decor2 = new Rectangle();
            decor2.Height = 3;
            decor2.Width = 7;
            decor2.Fill = Brushes.DarkGreen;
            decor2.Name = "B";
            //tree.Children.Add(decor2);
            this.trees.Children.Add(tree);
            //this.treesInGame[this.treesCount] = tree;
        }

        void spawnPuddle()
        {
            Canvas puddle = new Canvas();
            Rectangle main = new Rectangle();
            double height = 150;
            double width = 150;
            main.Fill = Brushes.Blue;
            main.Height = height;
            main.Width = width;
            puddle.Height = height;
            puddle.Width = width;
            puddle.Children.Add(main);
            for (int i1 = 0; i1 < 5; ++i1)
            {
                Rectangle land = new Rectangle();
                land.Height = rand.Next(30, 60);
                land.Width = rand.Next(30, 60);
                land.Fill = Brushes.Green;
                land.Name = "puddledecor";
                Canvas.SetTop(land, rand.Next(0, (int)puddle.Height - (int)land.Height));
                Canvas.SetLeft(land, rand.Next(0, (int)puddle.Width - (int)land.Width));
                if (i1 == 1)
                {
                    Canvas.SetTop(land, 0);
                    Canvas.SetLeft(land, 0);
                }
                else if (i1 == 2)
                {
                    Canvas.SetTop(land, 0);
                    Canvas.SetLeft(land, puddle.Width - land.Width);
                }
                else if (i1 == 3)
                {
                    Canvas.SetTop(land, puddle.Height - land.Height);
                    Canvas.SetLeft(land, 0);
                }
                else if (i1 == 4)
                {
                    Canvas.SetTop(land, puddle.Height - land.Height);
                    Canvas.SetLeft(land, puddle.Width - land.Width);
                }
                puddle.Children.Add(land);
            }
            Canvas.SetLeft(puddle, rand.Next(0, (int)this.window.Width - 50));
            Canvas.SetTop(puddle, rand.Next(0, (int)this.window.Height - 50));
            this.puddles.Children.Add(puddle);
        }

        private void treeclick(Canvas tree)
        {
            this.trees.Children.Remove(tree);
            this.spawnApple(tree);
            this.spawnApple(tree);
            this.spawnApple(tree);
        }

        void spawnSpeedup()
        {
            int speed = rand.Next(1, 3);
            Rectangle block = new Rectangle();
            block.Fill = Brushes.Pink;
            block.Height = 10;
            block.Width = 10;
            block.Name = $"speedup0{speed}";
            this.blocks.Children.Add(block);
            Canvas.SetLeft(block, rand.Next((int)this.window.Width - 50));
            Canvas.SetTop(block, rand.Next((int)this.window.Height - 50));
        }
        void spawnSlowdown()
        {
            int speed = rand.Next(1, 3);
            Rectangle block = new Rectangle();
            block.Fill = Brushes.DarkBlue;
            block.Height = 10;
            block.Width = 10;
            block.Name = $"slowdown0{speed}";
            this.blocks.Children.Add(block);
            Canvas.SetLeft(block, rand.Next((int)this.window.Width - 50));
            Canvas.SetTop(block, rand.Next((int)this.window.Height - 50));
        }

        void start()
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0.0, 0.0);
            brush.EndPoint = new Point(1.0, 0.0);
            GradientStop stop1 = new GradientStop(Colors.Red, 0.0);
            GradientStop stop2 = new GradientStop(Colors.Yellow, 0.5);
            GradientStop stop3 = new GradientStop(Colors.Green, 1.0);
            brush.GradientStops.Add(stop1);
            brush.GradientStops.Add(stop2);
            brush.GradientStops.Add(stop3);
            this.titleScreenText.Foreground = brush;
            this.welcomeScreenText.Foreground = brush;
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 1;
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            //playtimeText.BeginAnimation(OpacityProperty, anim);
            this.readProfiles();
        }

        void startGame()
        {
            this.isOnTitleScreen = false;
            this.hud.Visibility = Visibility.Visible;
            this.titleScreen.Visibility = Visibility.Hidden;
        }

        void hidePausedScreen()
        {
            //pausedScreen.Visibility = Visibility.Hidden;
            DoubleAnimation OpacityAnim = new DoubleAnimation();
            OpacityAnim.From = 0;
            OpacityAnim.To = 0.25;
            OpacityAnim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            DoubleAnimation OpacityAnim2 = new DoubleAnimation();
            OpacityAnim2.From = 0;
            OpacityAnim2.To = 1;
            OpacityAnim2.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            DoubleAnimation TopAnim = new DoubleAnimation();
            TopAnim.From = 400;
            //TopAnim.To = this.window.Height / 2 - resumeText.Height / 2 + 100;
            TopAnim.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            //pausedText.BeginAnimation(OpacityProperty, OpacityAnim2);
            //resumeText.BeginAnimation(TopProperty, TopAnim);
            //pausedscreenBackground.BeginAnimation(OpacityProperty, OpacityAnim);
        }

        void showScreenText(int stage, int type)
        {
            //pausedScreen.Visibility = Visibility.Visible;
            DoubleAnimation SizeAnim = new DoubleAnimation();
            if (stage == 1)
            {
                SizeAnim.From = 28;
                SizeAnim.To = 26.5;
                SizeAnim.Duration = new Duration(TimeSpan.FromSeconds(0.05));
            }
            else if (stage == 2)
            {
                SizeAnim.From = 26.5;
                SizeAnim.To = 25;
                SizeAnim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            }
            else if (stage == 3)
            {
                SizeAnim.From = 25;
                SizeAnim.To = 24;
                SizeAnim.Duration = new Duration(TimeSpan.FromSeconds(0.15));
                DoubleAnimation OpacityAnim = new DoubleAnimation();
                OpacityAnim.From = 0;
                OpacityAnim.To = 1;
                OpacityAnim.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                //resumeText.BeginAnimation(OpacityProperty, OpacityAnim);
            }
            else if (stage == 4)
            {
                SizeAnim.From = 24;
                SizeAnim.To = 23;
                SizeAnim.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            }
            if (stage <= 4)
            {
                int newStage = stage + 1;
                SizeAnim.Completed += (sender, EventArgs) => showScreenText(newStage, type);
                screenText.BeginAnimation(FontSizeProperty, SizeAnim);
            }
            if (stage == 5 && type == 2)
            {
                screenText.Content = null;
            }
        }

        void showEndScreen()
        {
            endScreenIsShown = true;
            endScreen.Visibility = Visibility.Visible;
            showScreenText(1, 2);
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 0.5;
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            anim.Completed += showEndString;
            endScreenBackground.BeginAnimation(OpacityProperty, anim);
        }

        private void showEndString(object sender, EventArgs e)
        {
            if (timePlayed < 100)
            {
                playtimeText.Content = $"You lasted {timePlayed} seconds... skill issue";
            }
            else
            {
                playtimeText.Content = $"You lasted {timePlayed} seconds...";
            }
            timePlayed = 0;
            endScreenText.Visibility = Visibility.Visible;
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 1;
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            playtimeText.BeginAnimation(OpacityProperty, anim);
            keybindRetryText.BeginAnimation(OpacityProperty, anim);
            keybindQuitText.BeginAnimation(OpacityProperty, anim);
        }

        void readProfiles()
        {
            int i1 = 0;
            long totalscore = 0;
            BrushConverter conv = new BrushConverter();
            try
            {
                foreach(string profile in Directory.GetDirectories($@"{this.gameDir}\profiles"))
                {
                    ++i1;
                    using (StreamReader reader = new StreamReader($@"{profile}\data.txt"))
                    {
                        string[] dataSplit = reader.ReadToEnd().Split(':');
                        totalscore = Convert.ToInt64(dataSplit[0]);
                    }
                    string final = profile.Replace($@"{this.gameDir}\profiles\", null);
                    Canvas listing = new Canvas();
                    Rectangle main = new Rectangle();
                    Rectangle source = new Rectangle();
                    Button but = new Button();
                    Label txt = new Label();
                    Label txtScore = new Label();
                    listing.Width = 190;
                    listing.Height = 50;
                    txt.Width = 190;
                    txt.Height = 50;
                    txt.VerticalContentAlignment = VerticalAlignment.Center;
                    txt.Content = final;
                    txt.FontWeight = FontWeights.Bold;
                    txt.FontSize = 21;
                    txt.Foreground = Brushes.White;
                    txtScore.Width = 190;
                    txtScore.Height = 50;
                    txtScore.VerticalContentAlignment = VerticalAlignment.Center;
                    txtScore.HorizontalContentAlignment = HorizontalAlignment.Right;
                    txtScore.Content = totalscore;
                    txtScore.FontWeight = FontWeights.Bold;
                    txtScore.FontSize = 16;
                    LinearGradientBrush brush = new LinearGradientBrush();
                    brush.StartPoint = new Point(0.0, 0.0);
                    brush.EndPoint = new Point(0.0, 1.0);
                    if (totalscore > 999)
                    {
                        GradientStop stop1 = new GradientStop(Colors.Yellow, 0.0);
                        GradientStop stop2 = new GradientStop(Colors.Orange, 0.3);
                        GradientStop stop3 = new GradientStop(Colors.DarkOrange, 0.6);
                        GradientStop stop4 = new GradientStop(Colors.Red, 1.0);
                        brush.GradientStops.Add(stop1);
                        brush.GradientStops.Add(stop2);
                        brush.GradientStops.Add(stop3);
                        brush.GradientStops.Add(stop4);
                        txtScore.Foreground = brush;
                    }
                    else
                    {
                        txtScore.Foreground = Brushes.White;
                    }
                    main.Width = 190;
                    main.Height = 50;
                    main.Fill = (Brush)conv.ConvertFromString("#FF0A0A0A");
                    but.Content = final;
                    but.Width = 190;
                    but.Height = 50;
                    but.FontSize = 48;
                    but.Opacity = 0;
                    but.Click += buttonClicked;
                    but.Name = "prof_select_btn";
                    listing.Children.Add(main);
                    listing.Children.Add(txt);
                    if (totalscore > 0)
                    {
                        listing.Children.Add(txtScore);
                    }
                    listing.Children.Add(but);
                    Canvas.SetLeft(listing, 5);
                    if (i1 == 1)
                    {
                        Canvas.SetTop(listing, 57);
                    }
                    else
                    {
                        Canvas.SetTop(listing, 57 * i1);
                    }
                    profiles.Children.Add(listing);
                }
                foreach (Canvas listing in profiles.Children)
                {
                    Canvas.SetTop(listing, Canvas.GetTop(listing) - 52);
                }
            }
            catch (Exception detering)
            {

            }
        }

        void readProfile(string name)
        {
            if (Directory.Exists($@"{this.gameDir}\profiles\{name}"))
            {
                using (StreamReader reader = new StreamReader($@"{this.gameDir}\profiles\{name}\data.txt"))
                {
                    string[] dataSplit = reader.ReadToEnd().Split(':');
                    this.totalscore = Convert.ToInt32(dataSplit[0]);
                    this.besttime = Convert.ToInt32(dataSplit[1]);
                    this.playername = name;
                    this.uiWelcome.Visibility = Visibility.Hidden;
                    this.titleScreen.Visibility = Visibility.Visible;
                    this.profileColorCode = dataSplit[2];
                    BrushConverter converter = new BrushConverter();
                    Brush b = (Brush)converter.ConvertFromString(dataSplit[2]);
                    this.level = Convert.ToInt64(dataSplit[3]);
                    this.exp = Convert.ToInt64(dataSplit[4]);
                    this.expr = Convert.ToInt64(dataSplit[5]);
                    this.levelText.Content = this.level;
                    this.nextLevelText.Content = this.level + 1;
                    this.expProgr.Maximum = this.expr;
                    this.player.Fill = b;
                    DoubleAnimation anim = new DoubleAnimation();
                    anim.From = this.expProgr.Value;
                    anim.To = this.exp;
                    anim.Duration = new Duration(TimeSpan.FromSeconds(0.25));
                    this.expProgr.BeginAnimation(ProgressBar.ValueProperty, anim);
                    Label nameLabel = new Label();
                    nameLabel.Width = 150;
                    nameLabel.Height = 50;
                    nameLabel.Content = this.playername;
                    nameLabel.FontWeight = FontWeights.Bold;
                    nameLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    nameLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    nameLabel.Foreground = Brushes.White;
                    nameLabel.FontSize = 16;
                    this.playerNameLabel = nameLabel;
                    this.namelabels.Children.Add(this.playerNameLabel);
                    if (this.level < 1000)
                    {
                        this.levelText.FontSize = 10;
                    }
                    if (this.level < 100)
                    {
                        this.levelText.FontSize = 12;
                    }
                    if (this.level < 10)
                    {
                        this.levelText.FontSize = 16;
                    }
                    if (this.level + 1 < 1000)
                    {
                        this.nextLevelText.FontSize = 10;
                    }
                    if (this.level + 1 < 100)
                    {
                        this.nextLevelText.FontSize = 12;
                    }
                    if (this.level + 1 < 10)
                    {
                        this.nextLevelText.FontSize = 16;
                    }
                }
            }
            else
            {
                Console.WriteLine($"System failed to read profile {name} because it could not be found.");
            }
        }

        void writeProfile(string name)
        {
            try
            {
                long score = 0;
                if (Directory.Exists($@"{this.gameDir}\profiles\{name}"))
                {
                    using (StreamReader reader = new StreamReader($@"{this.gameDir}\profiles\{name}\data.txt"))
                    {
                        string[] dataSplit = reader.ReadToEnd().Split(':');
                        score = Convert.ToInt64(dataSplit[0]);
                        score = score + this.score;
                    }
                }
                if (!Directory.Exists($@"{this.gameDir}\profiles\{name}"))
                {
                    Directory.CreateDirectory($@"{this.gameDir}\profiles\{name}");
                }
                if (Directory.Exists($@"{this.gameDir}\profiles\{name}"))
                {
                    using (StreamWriter writer = new StreamWriter($@"{this.gameDir}\profiles\{name}\data.txt"))
                    {
                        writer.Write($@"{score}:0:{this.profileColorCode}:{this.level}:{this.exp}:{this.expr}");
                    }
                }
                else
                {
                    Console.WriteLine($"System failed to write profile {name} because it could not be found.");
                }
            }
            catch (Exception e)
            {
                this.handleCrash($"System failed to write profile: {e.Message}");
            }
        }

        private void buttonClicked(object sender, RoutedEventArgs e)
        {
            Button bttn = (Button)sender;
            if (bttn == this.loginBtn)
            {
                readProfile(this.selectedprofile);
            }
            if (bttn == this.playBtn)
            {
                this.startGame();
            }
            if (bttn == this.quitBtn)
            {
                Environment.Exit(0);
            }
            if (bttn == this.createBtn)
            {
                this.createProfileName = this.createProfileNameBox.Text;
                if (this.createProfileNameBox.Text.Length < 11)
                {
                    this.createProfileName = this.createProfileNameBox.Text;
                    writeProfile(this.createProfileName);
                    this.uiCreate.Visibility = Visibility.Hidden;
                    this.uiWelcome.Visibility = Visibility.Visible;
                    this.profiles.Children.Clear();
                    this.readProfiles();
                }
            }
            if (bttn == this.createBtn1)
            {
                this.uiWelcome.Visibility = Visibility.Hidden;
                this.uiCreate.Visibility = Visibility.Visible;
                foreach(UIElement element in this.uiCreate.Children)
                {
                    if (element != this.createScreenBackground)
                    {
                        DoubleAnimation anim = new DoubleAnimation();
                        anim.From = 0;
                        anim.To = 1;
                        anim.Duration = new Duration(TimeSpan.FromSeconds(0.75));
                        element.BeginAnimation(OpacityProperty, anim);
                    }
                }
            }
            if (bttn.Name == "prof_select_btn")
            {
                try
                {
                    string name = (string)bttn.Content;
                    this.selectedprofile = name;
                    Canvas.SetTop(profSelectionEffect, Canvas.GetTop((Canvas)bttn.Parent));
                    DoubleAnimation anim = new DoubleAnimation();
                    anim.From = 1;
                    anim.To = 0;
                    anim.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                    profSelectionEffect.BeginAnimation(OpacityProperty, anim);
                    DoubleAnimation anim2 = new DoubleAnimation();
                    anim2.From = 0;
                    anim2.To = 100;
                    anim2.Duration = new Duration(TimeSpan.FromSeconds(0.25));
                    profSelectionEffect.BeginAnimation(ProgressBar.ValueProperty, anim2);
                }
                catch (Exception error)
                {

                }
            }
        }

        private void profcolorSelectorSelect(object sender, MouseButtonEventArgs e)
        {
            Rectangle color = (Rectangle)sender;
            Canvas.SetLeft(profcolorSelector, Canvas.GetLeft(color) - 2.5);
            Canvas.SetTop(profcolorSelector, Canvas.GetTop(color) - 2.5);
            this.profileColorCode = color.Fill.ToString();
            createPreview.Fill = color.Fill;
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 1;
            anim.To = 0;
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            previewEffect.BeginAnimation(OpacityProperty, anim);
            DoubleAnimation anim2 = new DoubleAnimation();
            anim2.From = 0;
            anim2.To = 100;
            anim2.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            previewEffect.BeginAnimation(ProgressBar.ValueProperty, anim2);
        }

        private void previewTextChanged(object sender, TextChangedEventArgs e)
        {
            previewName.Content = this.createProfileNameBox.Text;
        }

        private void btnMouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            DoubleAnimation animTop = new DoubleAnimation();
            animTop.From = Canvas.GetTop(this.buttonSelector);
            animTop.To = Canvas.GetTop((Canvas)btn.Parent);
            animTop.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            this.buttonSelector.BeginAnimation(TopProperty, animTop);
            DoubleAnimation animWidth = new DoubleAnimation();
            animWidth.From = this.buttonSelector.Width;
            animWidth.To = btn.Width;
            animWidth.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            this.buttonSelector.BeginAnimation(WidthProperty, animWidth);
        }

        void checkExp(long exp)
        {
            this.exp += exp;
            if (this.exp >= this.expr)
            {
                ++level;
                this.exp = 0;
                this.expr += this.expr % 10;
            }
            this.levelText.Content = this.level;
            this.nextLevelText.Content = this.level + 1;
            this.expProgr.Maximum = this.expr;
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = this.expProgr.Value;
            anim.To = this.exp;
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            this.expProgr.BeginAnimation(ProgressBar.ValueProperty, anim);
            if (this.level < 1000)
            {
                this.levelText.FontSize = 10;
            }
            if (this.level < 100)
            {
                this.levelText.FontSize = 12;
            }
            if (this.level < 10)
            {
                this.levelText.FontSize = 16;
            }
            if (this.level + 1 < 1000)
            {
                this.nextLevelText.FontSize = 10;
            }
            if (this.level + 1 < 100)
            {
                this.nextLevelText.FontSize = 12;
            }
            if (this.level + 1 < 10)
            {
                this.nextLevelText.FontSize = 16;
            }
        }

        void showNotification(string text)
        {
            this.isShowingNotif = true;
            this.notifText.Content = text;
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = this.window.Height;
            anim.To = this.window.Height - 77.5;
            anim.Duration = TimeSpan.FromSeconds(0.75);
            this.notif.BeginAnimation(TopProperty, anim);
        }

        void hideNotification()
        {
            this.isShowingNotif = false;
            this.notifTimer = 0;
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = Canvas.GetTop(this.notif);
            anim.To = this.window.Height;
            anim.Duration = TimeSpan.FromSeconds(0.75);
            this.notif.BeginAnimation(TopProperty, anim);
        }

        void handleCrash(string text)
        {
            errorscreen.Visibility = Visibility.Visible;
            errText.Content = text;
        }

        void generateTrees()
        {
            ++this.treesCount;
            this.spawnTree((int)this.playerX + rand.Next(-1000, 1000), (int)this.playerY + rand.Next(-1000, 1000));
        }
    }
}
