﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class GenerationLabirint
{
    /// <summary>
    /// На данной перегрузке работают все функции для генерации
    /// </summary>
    /// <param name="picture">изображение нужное для отрисовки</param>
    /// <param name="timer"> Таймер для создания подобии эволюции создания </param>
    /// <param name="StartPosition"> Начальная позиция генерации </param>
    /// <param name="RectSize"> Размер клетки </param>
    public GenerationLabirint(PictureBox picture, Timer timer, Point StartPosition, int RectSize)
    {
        pictureBox = picture;
        this.timer = timer;

        CreateSettings(StartPosition,RectSize);
    }
    /// <summary>
    /// На данной перегрузке работают все функции для генерации
    /// </summary>
    /// <param name="picture">изображение нужное для отрисовки</param>
    /// <param name="timer"> Таймер для создания подобии эволюции создания </param>
    /// <param name="StartPosition"> Начальная позиция генерации </param>
    /// <param name="RectSize"> Размер клетки </param>
    /// <param name="ColorWall"> Цвет заднего фона </param>
    /// <param name="ColorHead"> Цвет головы </param>
    public GenerationLabirint(PictureBox picture, Timer timer, Point StartPosition, int RectSize, Brush ColorWall, Brush ColorHead)
    {
        pictureBox = picture;
        this.timer = timer;

        this.ColorWall = ColorWall;
        this.ColorHead = ColorHead;
        CreateSettings(StartPosition, RectSize);
    }
    /// <summary>
    /// На данной перегрузке работает только функция полной генерации
    /// </summary>
    /// <param name="picture">изображение нужное для отрисовки</param>
    /// <param name="StartPosition"> Начальная позиция генерации </param>
    /// <param name="RectSize"> Размер клетки </param>
    /// <param name="ColorWall"> Цвет заднего фона </param>
    public GenerationLabirint(PictureBox picture, Point StartPosition, int RectSize, Brush ColorWall)
    {
        pictureBox = picture;

        this.ColorWall = ColorWall;
        CreateSettings(StartPosition, RectSize);
    }
    /// <summary>
    /// На данной перегрузке работает только функция полной генерации
    /// </summary>
    /// <param name="picture">изображение нужное для отрисовки</param>
    /// <param name="StartPosition"> Начальная позиция генерации </param>
    /// <param name="RectSize"> Размер клетки </param>
    public GenerationLabirint(PictureBox picture, Point StartPosition, int RectSize)
    {
        pictureBox = picture;
        CreateSettings(StartPosition, RectSize);
    }

    public Timer timer;
    private PictureBox pictureBox;

    Graphics g;

    private int RectSZ;

    private int RectX;
    private int RectY;

    private int[,] Screen;
    private Block[,] Blocks;

    private Random rand = new Random((int)DateTime.Now.Ticks);

    private Stack<Point> Stack = new Stack<Point>();

    private Point NowPosition = new Point(0, 0);
    private Point StartPos = new Point(0, 0);
    private Point LastPos = new Point(0, 0);

    private Brush ColorWall = Brushes.Brown;
    private Brush ColorHead = Brushes.Aqua;

    private void CreateSettings(Point StartPosition, int RectSize)
    {
        g = Graphics.FromImage(pictureBox.Image);

        RectSZ = RectSize;
        
        RectX = pictureBox.Width / RectSZ;       
        RectY = pictureBox.Height / RectSZ;  
        
        Screen = new int[RectX, RectY];
        Blocks = new Block[RectX, RectY];

        for (int i = 0; i < RectX; i++)
        {
            for (int j = 0; j < RectY; j++)
            {
                Blocks[i, j] = new Block(RectSZ, new Point(i, j));
            }
        }

        StartPos = StartPosition;
        NowPosition = StartPosition;
        LastPos = StartPosition;
    }
    /// <summary>
    /// Метод по тиковому созданию лабиринту 
    /// </summary>
    public void GenerationOnTick()
    {
        Screen[NowPosition.X, NowPosition.Y] = 1;
        List<Point> put = GetPoint();
        NearDraw();
        if (put.Count == 0)
        {
            if (Stack.Count == 0)
            {
                timer.Stop();
            }
            GoBackPosition();
            if (NowPosition == StartPos)
            {
                NearDraw();
                timer.Stop();
            }
            return;
        }

        GoNextPosition(put);
    }
    /// <summary>
    /// Полное создание лабиринта
    /// </summary>
    public void GenerationFull()
    {
        while (true)
        {
            Screen[NowPosition.X, NowPosition.Y] = 1;
            List<Point> put = GetPoint();
            if (put.Count == 0)
            {
                if (Stack.Count == 0)
                {
                    break;
                }
                GoBackPosition();
                if (NowPosition == StartPos)
                {
                    break;
                }
                continue;
            }
            GoNextPosition(put);
        }
        ForeachDraw();
    }

    private void GoBackPosition()
    {
        Stack.Pop();
        LastPos = NowPosition;
        NowPosition = Stack.Peek();
    }

    private void GoNextPosition(List<Point> put)
    {
        Stack.Push(NowPosition);
        Point Step = put[rand.Next(put.Count)];

        DeleteWall(Step);

        LastPos = NowPosition;

        NowPosition.X += Step.X;
        NowPosition.Y += Step.Y;
    }

    private void DeleteWall(Point Step)
    {
        if (Step.Y == 1)
        {
            Blocks[NowPosition.X + Step.X, NowPosition.Y + Step.Y].UP = false;
        }
        else if (Step.Y == -1)
        {
            Blocks[NowPosition.X, NowPosition.Y].UP = false;
        }
        else if(Step.X == 1)
        {
            Blocks[NowPosition.X + Step.X, NowPosition.Y + Step.Y].LEFT = false;
        }
        else if (Step.X == -1)
        {
            Blocks[NowPosition.X, NowPosition.Y].LEFT = false;
        }
    }
    private void NearDraw()
    {
        g.FillRectangle(ColorWall, LastPos.X * RectSZ, LastPos.Y * RectSZ, RectSZ, RectSZ);
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (ProvOutRangeArray(new Point(i, j)))
                    continue;
                Blocks[NowPosition.X + i, NowPosition.Y + j].Draw(g);
            }
        }

        g.FillRectangle(ColorHead, NowPosition.X * RectSZ, NowPosition.Y * RectSZ, RectSZ, RectSZ);

        pictureBox.Invalidate();
    }

    private void ForeachDraw()
    {
        g.FillRectangle(ColorWall, 0, 0, pictureBox.Width, pictureBox.Height);
        foreach (Block item in Blocks)
        {
            item.Draw(g);
        }
        pictureBox.Invalidate();
    }
    private bool ProvOutRangeArray(Point point) => point.X + NowPosition.X < 0 || point.X + NowPosition.X >= RectX || point.Y + NowPosition.Y < 0 || point.Y + NowPosition.Y >= RectY;
    private List<Point> GetPoint()
    {
        List<Point> put = new List<Point>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Math.Abs(i) == Math.Abs(j))
                    continue;

                if (ProvOutRangeArray(new Point(i, j)))
                    continue;

                if (Screen[NowPosition.X + i, NowPosition.Y + j] == 1)
                    continue;

                put.Add(new Point(i, j));
            }
        }
        return put;
    }
}

