using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiometriaOdciskuPalca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca.Tests
{
    [TestClass()]
    public class ImageSupporterTests
    {
        [TestMethod()]
        public void BitmapImage2BitmapTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteObjectTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Bitmap2BitmapImageTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void To8bitTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void convertToModuloBitmapTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void convertBitmapToImageTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void convertImageToBitmapimageTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void bitmapToBitmapimage2Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteHBitmapTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToBitmapSourceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ScaleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBitmapTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void WriteBitmapTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void matchAreaTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DegreeToRadianTest()
        {
            int degree = 180;

            double expected = Math.PI;

            double actual = ImageSupporter.DegreeToRadian(degree);

            Assert.AreEqual(expected,actual);
        }

        [TestMethod()]
        public void RadianToDegreeTest()
        {
            double radian = 0.45;

            double expectedDeggree = 25.783100781;

            double deegree = ImageSupporter.RadianToDegree(radian);
            Assert.IsTrue(Math.Abs(deegree-expectedDeggree)<0.001);
        }

        [TestMethod()]
        public void SaveTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void restrictAngleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void matchLineTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void matchLineTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void matchLineTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void matchLineTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void matchLine1Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void matchLine2Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EnumerateLineNoDiagonalStepsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void angletoIIIandIVTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getRandomColorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetLineTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ColorToGrayscaleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAreaTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAreaBitmapTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReverseBitmapTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GrayScaleToColorTest()
        {
            Assert.Fail();
        }
    }
}