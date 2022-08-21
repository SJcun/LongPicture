using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PictureShowDemo
{
    public partial class UCShowLongPicture : UserControl
    {
        #region 私有变量

        /// <summary>
        /// 图像地址
        /// </summary>
        private string FilePath { get; set; }

        /// <summary>
        /// 存储PictureBox控件的数组
        /// </summary>
        private PictureBox[] Boxes { get; set; }

        /// <summary>
        /// 鼠标是否点击的标记
        /// </summary>
        private bool isMouseDown = false;

        /// <summary>
        /// 图像宽度
        /// </summary>
        private int ImageWidth { get; set; }

        /// <summary>
        /// 图像高度
        /// </summary>
        private int ImageHeight { get; set; }

        /// <summary>
        /// 偏移，移动时尽量避免割裂感
        /// </summary>
        private int Offset = 2;

        /// <summary>
        /// 显示区域的左边界
        /// </summary>
        private int LeftBorder = 0;

        /// <summary>
        /// 显示区域的右边界
        /// </summary>
        private int RightBorder = 0;

        /// <summary>
        /// 记录鼠标按下时的点、三个PictureBox的起始点，判断平移的标记点
        /// </summary>
        private Point MousePoint, Pbx1Point, Pbx2Point, Pbx3Point, HalfPoint;

        /// <summary>
        /// 标记线
        /// </summary>
        private Point TagLine { get; set; }

        /// <summary>
        /// 用于PictureBox数组的记录索引
        /// </summary>
        private int BoxIndex1 = 0, BoxIndex2 = 1, BoxIndex3 = 2;

        #endregion

        #region 公有变量

        #endregion

        #region 构造器

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="boxWidth">待显示区域的宽度</param>
        /// <param name="boxHeight">待显示区域的高度</param>
        public UCShowLongPicture(int boxWidth, int boxHeight)
        {
            InitializeComponent();

            this.Height = boxHeight;
            this.Width = boxWidth;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置图像地址
        /// </summary>
        /// <param name="filePath">图像地址</param>
        /// <param name="width">显示出来的图像宽度</param>
        /// <param name="height">显示出来的图像高度</param>
        public void SetImageParameter(string filePath, int width, int height)
        {
            this.FilePath = filePath;
            this.ImageWidth = width;
            this.ImageHeight = height;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCShowLongPicture_Load(object sender, EventArgs e)
        {
            //声明三个PictureBox控件
            PictureBox pictureBox1 = new PictureBox();
            PictureBox pictureBox2 = new PictureBox();
            PictureBox pictureBox3 = new PictureBox();

            //存到数组里
            Boxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3 };

            //绑定事件
            BoxesBindEvent();

            //定义初始位置
            pictureBox2.Location = new Point(0,0);
            pictureBox1.Left = pictureBox2.Left - this.Width - Offset;
            pictureBox3.Left = pictureBox2.Right - Offset;

            //显示图像
            if(this.FilePath != null || !this.FilePath.Equals(""))
            {
                try
                {
                    Boxes[0].Image = null;
                    Boxes[1].Image = TailorImage(this.FilePath, 0, 0, this.ImageWidth, this.ImageHeight);
                    Boxes[2].Image = TailorImage(this.FilePath, this.ImageWidth, 0, this.ImageWidth, this.ImageHeight);
                    LeftBorder = 0 - this.ImageWidth;
                    RightBorder = this.ImageWidth * 2;
                }
                catch(Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

        /// <summary>
        /// 为PictureBox绑定事件
        /// </summary>
        private void BoxesBindEvent()
        {
            for (int index = 0; index < Boxes.Length; index++)
            {
                Boxes[index].Width = this.Width;
                Boxes[index].Height = this.Height;
                Boxes[index].SizeMode = PictureBoxSizeMode.StretchImage;
                this.Controls.Add(Boxes[index]);

                Boxes[index].MouseDown += new System.Windows.Forms.MouseEventHandler((object sender, MouseEventArgs e) => {   //鼠标点击事件
                    MouseDownCalculate();
                });

                Boxes[index].MouseMove += new System.Windows.Forms.MouseEventHandler((object sender, MouseEventArgs e) => {   //鼠标移动事件
                    MouseMoveCalculate();
                });

                Boxes[index].MouseUp += new System.Windows.Forms.MouseEventHandler((object sender, MouseEventArgs e) => {    //鼠标抬起事件
                    isMouseDown = false;
                });
            }
        }

        /// <summary>
        /// 鼠标按下时的处理
        /// </summary>
        private void MouseDownCalculate()
        {
            isMouseDown = true;
            MousePoint = PointToClient(Control.MousePosition);//记录鼠标坐标
            Pbx2Point = Boxes[BoxIndex2].Location;
            Pbx1Point = Boxes[BoxIndex1].Location;
            Pbx3Point = Boxes[BoxIndex3].Location;

            HalfPoint = new Point(Boxes[BoxIndex2].Location.X + (Boxes[BoxIndex2].Width / 2), 0);
        }

        /// <summary>
        /// 鼠标移动时的处理
        /// </summary>
        private void MouseMoveCalculate()
        {
            //鼠标坐标的相对改变值
            int a = PointToClient(Control.MousePosition).X - MousePoint.X;
            int b = PointToClient(Control.MousePosition).Y - MousePoint.Y;
            //图片坐标计算&赋值
            if (isMouseDown)
            {
                Boxes[BoxIndex2].Location = new Point(Pbx2Point.X + a, Pbx2Point.Y);//图片新的坐标 = 图片起始坐标 + 鼠标相对位移
                Boxes[BoxIndex1].Location = new Point(Pbx1Point.X + a, Pbx1Point.Y);
                Boxes[BoxIndex3].Location = new Point(Pbx3Point.X + a, Pbx3Point.Y);

                TagLine = new Point(HalfPoint.X + a, HalfPoint.Y);

                if (TagLine.X + Offset < this.Location.X)
                {
                    Boxes[BoxIndex1].Left = Boxes[BoxIndex3].Right - Offset;
                    LeftBorder += this.ImageWidth;
                    Boxes[BoxIndex1].Image = TailorImage(this.FilePath, RightBorder, 0, this.ImageWidth, this.ImageHeight);
                    RightBorder += this.ImageWidth;


                    BoxIndex1 = Add(BoxIndex1);
                    BoxIndex2 = Add(BoxIndex2);
                    BoxIndex3 = Add(BoxIndex3);
                    MouseDownCalculate();

                    HalfPoint = new Point(Boxes[BoxIndex2].Location.X + (Boxes[BoxIndex2].Width / 2), 0);

                    //Debug.Print("偏移后：" + BoxIndex1.ToString());
                }

                if (TagLine.X - Offset > (this.Location.X + this.Width))
                {
                    Boxes[BoxIndex3].Left = Boxes[BoxIndex1].Left - Boxes[BoxIndex1].Width + Offset;
                    LeftBorder -= this.ImageWidth;
                    RightBorder -= this.ImageWidth;
                    Boxes[BoxIndex3].Image = TailorImage(this.FilePath, LeftBorder, 0, this.ImageWidth, this.ImageHeight);

                    BoxIndex1 = Add(BoxIndex1, 2);
                    BoxIndex2 = Add(BoxIndex2, 2);
                    BoxIndex3 = Add(BoxIndex3, 2);
                    MouseDownCalculate();

                    HalfPoint = new Point(Boxes[BoxIndex2].Location.X + (Boxes[BoxIndex2].Width / 2), 0);

                    //Debug.Print("偏移后：" + BoxIndex1.ToString());
                }
            }
        }

        /// <summary>
        /// 适用于本程序的加法
        /// </summary>
        /// <param name="num1">被加数</param>
        /// <param name="num2">加数</param>
        /// <returns></returns>
        private int Add(int num1, int num2 = 1)
        {
            return (num1 + num2) % Boxes.Length;
        }

        /// <summary>
        /// 裁剪图像
        /// </summary>
        /// <param name="imagePath">图像地址</param>
        /// <param name="pointX">起始点x</param>
        /// <param name="pointY">起始点y</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns>裁剪好的图像</returns>
        private Image TailorImage(string imagePath, int pointX, int pointY, int width, int height)
        {
            Image originImage = Image.FromFile(imagePath);

            if (pointX < 0 || pointX > originImage.Width)
            {
                return null;
            }

            Rectangle cropRegion = new Rectangle(pointX, pointY, width, height);
            Bitmap result = new Bitmap(cropRegion.Width, cropRegion.Height);
            Graphics graphics = Graphics.FromImage(result);
            graphics.DrawImage(originImage, new Rectangle(0, 0, cropRegion.Width, cropRegion.Height), cropRegion, GraphicsUnit.Pixel);
            return result;
        }

        #endregion
    }
}
