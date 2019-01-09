using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class MinutiaWektorComperer
    {
        public int Limit { get; set; }//określenie ilości minucji które muszą zostać ze spobą dopasowane żeby uzyskać odciski za tożsame
        public int LimitMove { get; set; }//dopuszczlna odległość miedzy pikselami dla której uznaznaje się że piksele leżą w tym samym miejscu
        public double LimitRotate { get; set; }//określa dopuszczalny błąd obrotu przy którym kąty określa się jako toższame
        List<ModyficationElement> modyficationList;
        public MinutiaWektorComperer (int limit,int limitMove,double limitRotate)
        {
            this.Limit = limit;
            this.LimitMove = limitMove;
            this.LimitRotate = limitRotate;
            modyficationList = new List<ModyficationElement>();
            loadAngles();
            
        }

        //główna funkcja w klasie porównująca podobieństwo dwóch odciski palca na podstawie ich wektorów minucji
        //dane wejściowe: jeden z odcisków z bazy danych i nowy, analizowany odcisk
        //wyjście: true lub fals decyzja czy odciski są tożsame czy też nie
        public bool Compere(MinutiaWektor databaseMinutia,MinutiaWektor n )
        {
            Voting(databaseMinutia, n);
            ModyficationElement best=BestModyficationChecking();
            MinutiaWektor newMinutiaWektor = MapMinutiaWektor(n, best);

            int count = HowManyIdenticalMinuties(databaseMinutia, newMinutiaWektor);
            if (count >= this.Limit) return true;
            else return false;
        }

        private int HowManyIdenticalMinuties(MinutiaWektor databaseMinutia,MinutiaWektor n )
        {
            //tutaj można jeszcze zaprogramować zaznaczanie miucji jako dopasowanych
            int number=0;//liczna identycznych minucji
            foreach(var dm in databaseMinutia.m)
            {
                foreach(var nm in n.m)
                {
                    if(mm(dm,nm))
                    {
                        number++;
                        break;
                    }
                }
            }
            return number;
        }

        private MinutiaWektor MapMinutiaWektor(MinutiaWektor mw,ModyficationElement me)
        {
            MinutiaWektor newMinutiaWektor = new MinutiaWektor();
           foreach(var item in mw.m)
            {
                newMinutiaWektor.Add(MapMinutia(item, me));
            }

            return newMinutiaWektor;
        }

        private Minutia MapMinutia(Minutia m, ModyficationElement em)
        {
            int x =(int)(Math.Cos(em.angle) * m.p.X - Math.Sin(em.angle)*m.p.Y+em.x+0.5);
            int y=(int)(Math.Sin(em.angle) * m.p.X + Math.Cos(em.angle)*m.p.Y+em.y+0.5);
            double angle = m.direction + em.angle;
            return new Minutia(new Point(x,y), angle,m.kind);
        }
        private ModyficationElement BestModyficationChecking()
        {
            ModyficationElement tmp=modyficationList[0];

            foreach(var item in modyficationList)
            {
                if (item.vote > tmp.vote)
                    tmp = item;
            }
            return tmp;

        }

        List<int> angles = new List<int>();
        private void loadAngles()
        {
            for (int i = 0; i < 360; i++)
            {
                angles.Add(i);
            }
        }
        public class ModyficationElement
        {
            public int x{ get; set; }//przesunięcie względem osi X
            public int y{ get; set; }//przesunięcie względem osi Y
            public int angle{ get; set; }//obrót
            public int vote { get; set; }//ilość głosów oddanych na modyfikację

            public ModyficationElement(int x,int y,int angle)
            {
                this.x = x;
                this.y = y;
                this.angle = angle;
                this.vote = 0;
            
            }
           


            
                
          
        }

        public Tuple<bool,float> Voting(MinutiaWektor froDatabase, MinutiaWektor scaned)
        {
            foreach (int angle in angles)
            {
                foreach (var d in froDatabase.m)//wekrot minucji z bazy danych
                {
                    foreach (var n in scaned.m)//wektor minucji analizowanego odcisku
                    {
                        if(dd(n.direction+ImageSupporter.DegreeToRadian(angle),d.direction) <LimitRotate)
                        {
                            Point tmp = getShift(n,d, ImageSupporter.DegreeToRadian(angle));
                            ModyficationElement vote = new ModyficationElement(tmp.X,tmp.Y,angle);
                            Vote(vote);
                        }
                    }
                }
            }
            return new Tuple<bool, float>(false, 0);
        }

        public double sd(Minutia m1,Minutia m2)
        {
            return Math.Sqrt(Math.Pow(m1.p.X - m2.p.X, 2) + Math.Pow(m1.p.Y - m2.p.Y, 2));
        }

        public double dd(double angleNew,double angleOld)
        {
            return Math.Min(Math.Abs(angleOld - angleNew), ImageSupporter.DegreeToRadian(360) - Math.Abs(angleOld - angleNew));
        }
        //metoda zwracająca wartość przesunięcia obrazu,  aby dwie minucje pokryły się na obrazie już obruconym o zandany kąt q
        public  Point getShift(Minutia n,Minutia d, double q)//miara kątowa jest tu w stopniach trzeba będzie je zamienić na radiany, d to minucja z wektora bazy minucji z bazy danych
                                                                //  n to minucja z wektora minucji niwej analizowanje minucji
        {
            int deltaX = (int)(d.p.X - (Math.Cos(q) * n.p.X - Math.Sin(q) * n.p.Y) + 0.5);
            int deltaY = (int)(d.p.Y - (Math.Sin(q) * n.p.X + Math.Cos(q) * n.p.Y) + 0.5);
            return new Point(deltaX,deltaY);
        }

        public bool mm(Minutia m, Minutia prim)
        {
            if (sd(m, prim) <= LimitMove && dd(m.direction, prim.direction) <= LimitRotate)
            {
                return true;
            }
            else return false;
        }

        public void dyskretyzation(ModyficationElement n)//głosuje za przekształconą minucję w dyskretyzowane miejsce
        {

        }
        private void Vote(ModyficationElement me)
        {
            dyskretyzation(me);

            ModyficationListVote(me,1);
        }

        private void ModyficationListVote(ModyficationElement me,int n)
        {
            bool flag = true;
            foreach (var item in modyficationList)
            {
                if (item.x == me.x && item.y == me.y && item.angle == me.angle)
                {
                    item.vote += n;
                    flag =false;
                    break;
                }

            }
            if(flag)
            {
                me.vote = n;
                modyficationList.Add(me);  
            }

        }

        

        

        

    }
}
