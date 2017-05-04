using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Game1
{
    public class MyObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public object texture { get; set; }
        public Color cc { get; set; }

        public bool canCheck { get; set; }
        public bool canBreak { get; set; }
        public bool canReflection { get; set; }

        public event Action<object> CheckEvent;
        public event Action<object> BreakEvent;
        public event Func<object, object, bool> ReflectionEvent;

        public void OnBreak()
        {
            if(BreakEvent != null)
            {
                BreakEvent(this);
            }
        }

        public void OnCheck()
        {
            CheckEvent.Invoke(this);
        }

        public bool OnReflection(object obj)
        {
            if(ReflectionEvent != null)
            {
                return ReflectionEvent(this, obj);
            }
            return false;
        }

        public MyObject(object tt)
        {
            texture = tt;
        }

        public event Action<object,object> showHandler;
        public void Show()
        {
            showHandler.Invoke(texture, this);
        }
    }

    public class MyBall : MyObject
    {
        private float Speed { get; set; }
        private double _degree;
        private double Degree
        {
            get { return _degree; }
            set
            {
                if(_degree != value)
                {
                    _degree = value;
                    vx = Speed * Math.Cos(radians(_degree));
                    vy = Speed * Math.Sin(radians(_degree));
                }
            }
        }

        private double radians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        private double vx;
        private double vy;
        public MyBall(int x, int y, int size, float sp, Color c, object tt) : base(tt)
        {
            X = x;
            Y = y;
            Width = size;
            Height = size;
            cc = c;
            Speed = sp;

            Degree = 45;

            canCheck = true;
            canBreak = false;
            canReflection = false;
        }

        public void Reflection(int val)
        {
            switch (val)
            {
                case 1:
                    Degree = -Degree;
                    break;
                case 2:
                    Degree = 180 - Degree;
                    break;
                default:
                    break;
            }
        }

        public void Update(float elapsedTime)
        {
            X += (float)(vx * elapsedTime);
            Y += (float)(vy * elapsedTime);
            OnCheck();
        }
    }

    public class MyBoard : MyObject
    {
        private int BoundL { get; set; }
        private int BoundR { get; set; }
        private int BoundU { get; set; }
        private int BoundD { get; set; }
        private float Speed { get; set; }
        public MyBoard(int x, int y, int w, int h, Color c, float sp, int bl, int br, object tt): base(tt)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            cc = c;
            this.Speed = sp;
            this.BoundL = bl;
            this.BoundR = br;

            canCheck = false;
            canBreak = false;
            canReflection = true;
        }

        public void MoveLeft()
        {
            this.X -= (int)this.Speed;
            if (X < BoundL) X = BoundL;
        }

        public void MoveRight()
        {
            this.X += (int)this.Speed;
            if ((X+Width) > BoundR) X = BoundR-Width;
        }
    }

    public class MyWall : MyObject
    {
        public MyWall(int x, int y, int w, int h, Color c, object tt) : base(tt)
        {
            this.X = x;
            Y = y;
            Width = w;
            Height = h;
            cc = c;

            canCheck = false;
            canBreak = false;
            canReflection = true;
        }
    }

    public class MyFont : MyObject
    {
        public String ss { get; set; }
        public readonly int sX;
        public readonly int sY;
        public readonly Color sC;
        public MyFont(int x, int y, string s, Color c, int offset, object tt) : base(tt)
        {
            X = x;
            sX = x + offset;
            Y = y;
            sY = y + offset;
            cc = c;
            sC = new Color((byte)(cc.R * 0.5), (byte)(cc.G * 0.5), (byte)(cc.B * 0.5), (byte)(cc.A * 0.5));
            ss = s;

            canCheck = false;
            canBreak = false;
            canReflection = false;
        }
    }

    public class MyBrick: MyObject
    {
        public MyBrick(int x, int y, int w, int h, Color c, object tt) : base(tt)
        {
            this.X = x;
            Y = y;
            Width = w;
            Height = h;
            cc = c;

            canCheck = false;
            canBreak = true;
            canReflection = true;
        }
    }

    public class MyObjList : ObservableCollection<MyObject>
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    MyObject temp = item as MyObject;
                    if (temp == null) continue;
                    
                    if(temp.canBreak == true)
                    {
                        temp.BreakEvent += Temp_BreakEvent;
                    }
                    if (temp.canCheck == true)
                    {
                        temp.CheckEvent += Temp_CheckEvent;
                    }
                    if (temp.canReflection == true)
                    {
                        temp.ReflectionEvent += Temp_ReflectionEvent;
                    }
                }
            }
        }

        private bool Temp_ReflectionEvent(object src, object dist)
        {
            MyBall ball = dist as MyBall;
            MyObject trig = src as MyObject;
            if (ball == null || trig == null) return false;

            Vector2 pointL, pointR, pointT, pointB;
            pointL.X = ball.X;
            pointL.Y = ball.Y + ball.Height / 2;
            pointR.X = ball.X + ball.Width;
            pointR.Y = ball.Y + ball.Height / 2;

            pointT.X = ball.X + ball.Width / 2;
            pointT.Y = ball.Y;
            pointB.X = ball.X + ball.Width / 2;
            pointB.Y = ball.Y + ball.Height;



            if ( (pointL.X > trig.X) && (pointL.X <= trig.X+trig.Width) && 
                (pointL.Y > trig.Y) && (pointL.Y <= trig.Y+trig.Height) )
            {
                ball.Reflection(2);
                ball.X += (trig.X + trig.Width) - (int)pointL.X;
                return true;
            }
            else if ((pointR.X > trig.X) && (pointR.X <= trig.X + trig.Width) &&
                (pointR.Y > trig.Y) && (pointR.Y <= trig.Y + trig.Height))
            {
                ball.Reflection(2);
                ball.X += (trig.X) - (int)pointR.X;
                return true;
            }
            else if ((pointT.X > trig.X) && (pointT.X <= trig.X + trig.Width) &&
                (pointT.Y > trig.Y) && (pointT.Y <= trig.Y + trig.Height))
            {
                ball.Reflection(1);
                ball.Y += (trig.Y + trig.Height) - (int)pointT.Y;
                return true;
            }
            else if ((pointB.X > trig.X) && (pointB.X <= trig.X + trig.Width) &&
                (pointB.Y > trig.Y) && (pointB.Y <= trig.Y + trig.Height))
            {
                ball.Reflection(1);
                ball.Y += (trig.Y) - (int)pointB.Y;
                return true;
            }
            return false;

        }

        private void Temp_CheckEvent(object obj)
        {
            MyObject ball = obj as MyObject; //球
            if (ball == null) return;
            //檢查碰撞對象
            foreach (var item in this)
            {
                //檢查能否反射球
                bool tempBool =  item.OnReflection(ball);

                //如果有碰撞發生
                if (tempBool == true)
                {
                    //檢查能否消失
                    item.OnBreak();
                    break;
                }
            }

        }

        private void Temp_BreakEvent(object obj)
        {
            this.Remove((MyObject)obj);
        }
    }
}
