// Це дещо видозмінений приклад із книги Троелсена для ілюстрування
// поняття делегат і роботи з делегатами

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab11
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // --------- Клас Car ---------
        public class Car
        {
            public int CurrentSpeed { get; set; }
            public int MaxSpeed { get; set; }
            public string PetName { get; set; }

            private bool carIsDead;  // поле для перевірки, чи автомобіль не зламався

            public Car()             // конструктор класу
            {
                MaxSpeed = 100;
            }

            public Car(string name, int maxSp, int currSp)  // конструктор з параметрами
            {
                MaxSpeed = maxSp;
                CurrentSpeed = currSp;
                PetName = name;
            }

            // Оголошення делегата у класі Car
            public delegate void CarEngineHandler(string msgForCaller);

            // Оголошення закритої змінної listOfHandlers типу делегат
            CarEngineHandler listOfHandlers;

            // Додавання методу для доступу до змінної listOfHandlers ззовні класу
            public void RegisterWithCarEngine(CarEngineHandler metodToCall)
            {
                /* Змінній типу делегат присвоюємо метод, що має сигнатуру,
                 * яка вказана при оголошенні делегата */
                listOfHandlers = metodToCall;
            }

            /* Метод для зміни поточної швидкості автомобіля.
             * Він буде викликати процес створення повідомлення і додавання його до тексту мітки.
             * Залежно від швидкості автомобіля, будуть генеруватись різні повідомлення
             */
            public void Accselerate(int delta)
            {
                if (carIsDead)
                {
                    /* Змінна типу делегат listOfHandlers запускає метод, вказаний при її створенні
                     * з параметрами, заданими в операторі звертання до змінної.
                     * Перевіримо, чи передано метод у змінну listOfHandlers і
                     * якщо так, то викликаємо метод, адреса якого записана у змінній listOfHandlers
                     */
                    if (listOfHandlers != null)
                        listOfHandlers("Увага! Занадто велика швидкість!");
                }
                else
                {
                    CurrentSpeed += delta;

                    // Перевіряємо, чи передано метод у змінну listOfHandlers, а також
                    // чи не занадто велика швидкість і якщо так, то видаємо повідомлення
                    if ((MaxSpeed - CurrentSpeed < 10) && listOfHandlers != null)
                    {
                        // Викликаємо метод, записаний у делегаті за допомогою змінної типу делегат
                        // з новими параметрами
                        listOfHandlers("Увага! Занадто велика швидкість!");
                        listOfHandlers("На жаль автомобіль зламався");
                    }
                    else
                    {
                        if (CurrentSpeed >= MaxSpeed)
                            carIsDead = true;
                        else
                            listOfHandlers?.Invoke("Поточна швидкість=" + CurrentSpeed.ToString());
                    }
                }
            }
        }

        // Метод-обробник, який викликає делегат — виводить текст у label1
        public void OnCarEngineEvent(string msg)
        {
            label1.Text = label1.Text + msg + " \n";
        }

        // Обробник кліку по кнопці Start
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = ""; // очищаємо перед новим запуском

            // Створюємо об'єкт типу Car
            Car myCar = new Car("Старенький Запорожець", 100, 0);

            // Створимо змінну типу делегат і зашлемо в неї адресу методу,
            // який буде викликатись через цю змінну
            Car.CarEngineHandler myDelegat =
                new Car.CarEngineHandler(OnCarEngineEvent);

            /* Звернемось до методу RegisterWithCarEngine, щоб вказати метод,
             * який повинен викликатись (зареєструвати)
             */
            myCar.RegisterWithCarEngine(myDelegat);

            // Ми можемо викликати метод OnCarEngineEvent і поза делегатом
            OnCarEngineEvent("Старенький Запорожець");
            OnCarEngineEvent("Вмикаємо запалювання");

            // Змінюємо швидкість автомобіля і відслідковуємо, що трапиться
            for (int i = 0; i < 10; i++)
                myCar.Accselerate(10);
        }
    }
}
