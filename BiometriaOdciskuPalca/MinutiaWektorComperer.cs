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

       // public Dictionary<int ,Dictionary<int ,Dictionary<int,int>>> acumulator;
         //List<ModyficationElement> modyficationList;
        int[ , , ] acumulator;

        int angleRangeBottom = -46;
        int angleRangeTop = +46;
        int angleJump = 2;

        int xRangeBottom = -201;
        int xRangeTop = 200;
        int xJump = 3;

        int yRangeBottom = -201;
        int yRangeTop = 200;
        int yJump = 3;

        public MinutiaWektorComperer (int limit,int limitMove,double limitRotate)
        {
            this.Limit = limit;
            this.LimitMove = limitMove;
            this.LimitRotate = limitRotate;
          //  modyficationList = new List<ModyficationElement>();
           // loadAngles();
            LoadAcumulator();
           
            
        }

        public MinutiaWektorComperer()
        {

        }

        //główna funkcja w klasie porównująca podobieństwo dwóch odciski palca na podstawie ich wektorów minucji
        //dane wejściowe: jeden z odcisków z bazy danych i nowy, analizowany odcisk
        //wyjście: true lub fals decyzja czy odciski są tożsame czy też nie
        public Tuple<bool,int,ModyficationElement> Compere(MinutiaWektor databaseMinutia,MinutiaWektor n )
        {
            Voting(databaseMinutia, n);
            //ModyficationElement best=BestModyficationChecking();
            Tuple<ModyficationElement,int> tuple=BestModyficationChecking();
            ModyficationElement best = tuple.Item1;
            //int voteScore = tuple.Item2;

             MinutiaWektor newMinutiaWektor = MapMinutiaWektor(n, best);

             int count = HowManyIdenticalMinuties(databaseMinutia, newMinutiaWektor);
              return new Tuple<bool, int,ModyficationElement>(true,count,best);
           //  if (count >= this.Limit) return new Tuple<bool, int,ModyficationElement>(true,count,best);
            // else return new Tuple<bool, int,ModyficationElement>(false, count,best);

           // if (voteScore >= this.Limit) return new Tuple<bool, int,ModyficationElement>(true, voteScore,best);
          //  else return new Tuple<bool, int,ModyficationElement>(false, voteScore,best);
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
                   //     break;
                    }
                }
            }
            return number;
        }

        public MinutiaWektor MapMinutiaWektor(MinutiaWektor mw,ModyficationElement me)
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
            double angle = ImageSupporter.DegreeToRadian(em.angle);
            int x =(int)(Math.Cos(angle) * m.p.X - Math.Sin(angle)*m.p.Y+em.x+0.5);
            int y=(int)(Math.Sin(angle) * m.p.X + Math.Cos(angle)*m.p.Y+em.y+0.5);
            double angle2 = m.direction + angle;
            return new Minutia(new Point(x,y), angle2,m.kind);
        }
        private Tuple<ModyficationElement,int> BestModyficationChecking()
        {

           // ModyficationElement tmp;//=modyficationList[0];


            /*  foreach(var item in modyficationList)
              {
                  if (item.vote > tmp.vote)
                      tmp = item;
              }*/
            int bestVoted = 0;
            int choseAngle = 0, choseX = 0,choseY = 0;
            /*for (int angle = angleRangeBottom; angle < angleRangeTop; angle += angleJump)
            {
                for (int x = xRangeBottom; x < xRangeTop; x += xJump)
                {
                    for (int y = yRangeBottom; y < yRangeTop; y += yJump)
                    {

                        if (bestVoted < acumulator[angle, y, x])
                        {
                            bestVoted = acumulator[angle, y, x];
                            choseAngle = angle;
                            choseX = x;
                            choseY = y;
                            
                        }
                            
                    }
                }
            }*/

            for (int angle = 0; angle < (angleRangeTop - angleRangeBottom) / angleJump; angle += 1)
            {
                for (int x = 0; x < (xRangeTop - xRangeBottom) / xJump; x += 1)
                {
                    for (int y = 0; y < (yRangeTop - yRangeBottom) / yJump; y += 1)
                    {
                        if (bestVoted < acumulator[angle, y, x])
                        {

                        if(acumulator[angle, y, x]>10)
                            {

                            }
                            bestVoted = acumulator[angle, y, x];
                            choseAngle = angle;
                            choseX = x;
                            choseY = y;
                        }
                    }
                }
            }

            choseAngle =angleRangeBottom+choseAngle*angleJump;
            choseX =  xRangeBottom+ choseX*xJump;
            choseY = yRangeBottom+ choseY*yJump;





          //  return new ModyficationElement(choseAngle, choseX, choseY);
            return new Tuple<ModyficationElement,int>(new  ModyficationElement(choseAngle, choseX, choseY),bestVoted);
        }

        List<int> angles = new List<int>();
        private void loadAngles()
        {
            for (int i = -90; i < 90; i++)
            {
                angles.Add(i);
            }
        }
       /* public class ModyficationElement
        {
            public int x{ get; set; }//przesunięcie względem osi X
            public int y{ get; set; }//przesunięcie względem osi Y
            public int angle{ get; set; }//obrót
     //       public int vote { get; set; }//ilość głosów oddanych na modyfikację

            public ModyficationElement(int x,int y,int angle)
            {
                this.x = x;
                this.y = y;
                this.angle = angle;
               // this.vote = 0;
            
            }
           


            
                
          
        }*/

        public Tuple<bool,float> Voting(MinutiaWektor froDatabase, MinutiaWektor scaned)
        {
            for (int angle = angleRangeBottom; angle < angleRangeTop; angle += angleJump)
            {

                foreach (var d in froDatabase.m)//wekrot minucji z bazy danych
                {
                    foreach (var n in scaned.m)//wektor minucji analizowanego odcisku
                    {
                       // if(dd(ImageSupporter.DegreeToRadian( angle),0)<ImageSupporter.DegreeToRadian(3) && d.p.Equals(n.p))
                        if (dd(n.direction + ImageSupporter.DegreeToRadian(angle), d.direction) < LimitRotate)
                        {
                            Point tmp = getShift(n, d, ImageSupporter.DegreeToRadian(angle));
                            ModyficationElement vote = new ModyficationElement(tmp.X, tmp.Y, angle);
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

        public void LoadAcumulator()
        {
            acumulator = new int[ (angleRangeTop-angleRangeBottom)/angleJump+1,(xRangeTop-xRangeBottom)/xJump+1, (yRangeTop-yRangeBottom)/yJump+1 ];
            
            for(int angle=0;angle<(angleRangeTop-angleRangeBottom)/angleJump;angle+=1)
            {
                for(int x=0;x<(xRangeTop-xRangeBottom)/xJump;x+=1)
                {
                    for (int y = 0; y < (yRangeTop-yRangeBottom)/yJump ; y +=1)
                    {
                        acumulator[angle, y, x] = 0;
                    }
                }
            }




        }
        private void Vote(ModyficationElement me)
        {
            dyskretyzation(me);
            ModyficationListVote(me,1);
        }

        private void ModyficationListVote(ModyficationElement me, int n)//n oznacza ilość głósów
        {
            if (me.angle < angleRangeTop && me.angle > angleRangeBottom && me.x < xRangeTop && me.x > xRangeBottom && me.y < yRangeTop && me.y > yRangeBottom)
            { 
            int accurateAngleCell =(me.angle - angleRangeBottom) / angleJump;

            int accuratexCell= (me.x - xRangeBottom) / xJump;


            int accurateyCell =  (me.y - yRangeBottom) / yJump;
            // na wartość ustalonej komórki głosuje się 2 razy a na komórki okalające ją po jednym razie
            acumulator[accurateAngleCell, accuratexCell, accurateyCell] += 2; //

                try
                {
                    acumulator[accurateAngleCell, accuratexCell - 1, accurateyCell - 1] += 1; // 
                    acumulator[accurateAngleCell, accuratexCell - 1, accurateyCell] += 1; // 
                    acumulator[accurateAngleCell, accuratexCell, accurateyCell - 1] += 1; // 
                    acumulator[accurateAngleCell, accuratexCell + 1, accurateyCell + 1] += 1; // 
                    acumulator[accurateAngleCell, accuratexCell + 1, accurateyCell] += 1; // 
                    acumulator[accurateAngleCell, accuratexCell, accurateyCell + 1] += 1; // 
                    acumulator[accurateAngleCell, accuratexCell + 1, accurateyCell - 1] += 1; // 
                    acumulator[accurateAngleCell, accuratexCell - 1, accurateyCell + 1] += 1; // 
                }
                catch(IndexOutOfRangeException ex)
                {

                }
                }





           /* bool flag = true;
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
            }*/

        }

        

        

        

    }
}
