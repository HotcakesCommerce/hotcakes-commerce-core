#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Hotcakes.Web.Barcodes
{

    #region Enums

    public enum TYPE
    {
        UNSPECIFIED,
        UPCA,
        UPCE,
        UPC_SUPPLEMENTAL_2DIGIT,
        UPC_SUPPLEMENTAL_5DIGIT,
        EAN13,
        EAN8,
        Interleaved2of5,
        Standard2of5,
        Industrial2of5,
        CODE39,
        CODE39Extended,
        Codabar,
        PostNet,
        BOOKLAND,
        ISBN,
        JAN13,
        MSI_Mod10,
        MSI_2Mod10,
        MSI_Mod11,
        MSI_Mod11_Mod10,
        Modified_Plessey,
        CODE11,
        USD8,
        UCC12,
        UCC13,
        LOGMARS,
        CODE128,
        CODE128A,
        CODE128B,
        CODE128C,
        ITF14,
        CODE93
    }

    public enum SaveTypes
    {
        JPG,
        BMP,
        PNG,
        GIF,
        TIFF,
        UNSPECIFIED
    }

    #endregion

    /// <summary>
    ///     This class was designed to give developers and easy way to generate a barcode image from a string of data.
    /// </summary>
    [Serializable]
    public class Barcode : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw new Exception("EDISPOSE-1: " + ex.Message);
            }
        }

        #endregion

        #region Variables

        private string Raw_Data = string.Empty;
        private string Formatted_Data = string.Empty;
        private string Encoded_Value = string.Empty;
        private string _Country_Assigning_Manufacturer_Code = "N/A";
        private TYPE Encoded_Type = TYPE.UNSPECIFIED;
        private Color _ForeColor = Color.Black;
        private Color _BackColor = Color.White;
        private int _Width = 300;
        private int _Height = 150;
        private bool _IncludeLabel;
        private double _EncodingTime;
        private string _XML = string.Empty;
        private ImageFormat _ImageFormat = ImageFormat.Jpeg;

        #endregion

        #region Constructors

        /// <summary>
        ///     Default constructor.  Does not populate the raw data.  MUST be done via the RawData property before encoding.
        /// </summary>
        public Barcode()
        {
            EncodedImage = null;
        }

        /// <summary>
        ///     Constructor. Populates the raw data. No whitespace will be added before or after the barcode.
        /// </summary>
        /// <param name="data">String to be encoded.</param>
        public Barcode(string data)
        {
            EncodedImage = null;
            Raw_Data = data;
        }

        public Barcode(string data, TYPE iType)
        {
            EncodedImage = null;
            Raw_Data = data;
            Encoded_Type = iType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the formatted data.
        /// </summary>
        public string FormattedData
        {
            get { return Formatted_Data; }
        }

        /// <summary>
        ///     Gets or sets the raw data to encode.
        /// </summary>
        public string RawData
        {
            get { return Raw_Data; }
            set { Raw_Data = value; }
        }

        /// <summary>
        ///     Gets the encoded value.
        /// </summary>
        public string EncodedValue
        {
            get { return Encoded_Value; }
        }

        /// <summary>
        ///     Gets the Country that assigned the Manufacturer Code.
        /// </summary>
        public string Country_Assigning_Manufacturer_Code
        {
            get { return _Country_Assigning_Manufacturer_Code; }
        }

        /// <summary>
        ///     Gets or sets the Encoded Type (ex. UPC-A, EAN-13 ... etc)
        /// </summary>
        public TYPE EncodedType
        {
            set { Encoded_Type = value; }
            get { return Encoded_Type; }
        }

        /// <summary>
        ///     Gets the Image of the generated barcode.
        /// </summary>
        public Image EncodedImage { get; private set; }

        /// <summary>
        ///     Gets or sets the color of the bars. (Default is black)
        /// </summary>
        public Color ForeColor
        {
            get { return _ForeColor; }
            set { _ForeColor = value; }
        }

        /// <summary>
        ///     Gets or sets the background color. (Default is white)
        /// </summary>
        public Color BackColor
        {
            get { return _BackColor; }
            set { _BackColor = value; }
        }

        /// <summary>
        ///     Gets or sets the width of the image to be drawn. (Default is 300 pixels)
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        /// <summary>
        ///     Gets or sets the height of the image to be drawn. (Default is 150 pixels)
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        /// <summary>
        ///     Gets or sets whether a label should be drawn below the image.
        /// </summary>
        public bool IncludeLabel
        {
            set { _IncludeLabel = value; }
            get { return _IncludeLabel; }
        }

        /// <summary>
        ///     Gets or sets the amount of time in milliseconds that it took to encode and draw the barcode.
        /// </summary>
        public double EncodingTime
        {
            get { return _EncodingTime; }
            set { _EncodingTime = value; }
        }

        /// <summary>
        ///     Gets the XML representation of the Barcode data and image.
        /// </summary>
        public string XML
        {
            get { return _XML; }
        }

        /// <summary>
        ///     Gets or sets the image format to use when encoding and returning images. (Jpeg is default)
        /// </summary>
        public ImageFormat ImageFormat
        {
            get { return _ImageFormat; }
            set { _ImageFormat = value; }
        }

        #endregion

        #region Functions

        #region General Encode

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="StringToEncode">Raw data to encode.</param>
        /// <param name="Width">Width of the resulting barcode.(pixels)</param>
        /// <param name="Height">Height of the resulting barcode.(pixels)</param>
        /// <returns>Image representing the barcode.</returns>
        public Image Encode(TYPE iType, string StringToEncode, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            return Encode(iType, StringToEncode);
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="StringToEncode">Raw data to encode.</param>
        /// <param name="ForeColor">Foreground color</param>
        /// <param name="BackColor">Background color</param>
        /// <param name="Width">Width of the resulting barcode.(pixels)</param>
        /// <param name="Height">Height of the resulting barcode.(pixels)</param>
        /// <returns>Image representing the barcode.</returns>
        public Image Encode(TYPE iType, string StringToEncode, Color ForeColor, Color BackColor, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            return Encode(iType, StringToEncode, ForeColor, BackColor);
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="StringToEncode">Raw data to encode.</param>
        /// <param name="ForeColor">Foreground color</param>
        /// <param name="BackColor">Background color</param>
        /// <returns>Image representing the barcode.</returns>
        public Image Encode(TYPE iType, string StringToEncode, Color ForeColor, Color BackColor)
        {
            this.BackColor = BackColor;
            this.ForeColor = ForeColor;
            return Encode(iType, StringToEncode);
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="StringToEncode">Raw data to encode.</param>
        /// <returns>Image representing the barcode.</returns>
        public Image Encode(TYPE iType, string StringToEncode)
        {
            Raw_Data = StringToEncode;
            return Encode(iType);
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        internal Image Encode(TYPE iType)
        {
            Encoded_Type = iType;
            return Encode();
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.
        /// </summary>
        internal Image Encode()
        {
            var dtStartTime = DateTime.Now;

            //make sure there is something to encode
            if (string.IsNullOrEmpty(Raw_Data))
                throw new Exception("EENCODE-1: Input data not allowed to be blank.");

            if (EncodedType == TYPE.UNSPECIFIED)
                throw new Exception("EENCODE-2: Symbology type not allowed to be unspecified.");

            Encoded_Value = string.Empty;
            _Country_Assigning_Manufacturer_Code = "N/A";

            IBarcode ibarcode;
            switch (Encoded_Type)
            {
                case TYPE.UCC12:
                case TYPE.UPCA: //Encode_UPCA();
                    ibarcode = new UPCA(Raw_Data);
                    break;
                case TYPE.UCC13:
                case TYPE.EAN13: //Encode_EAN13();
                    ibarcode = new EAN13(Raw_Data);
                    break;
                case TYPE.Interleaved2of5: //Encode_Interleaved2of5();
                    ibarcode = new Interleaved2of5(Raw_Data);
                    break;
                case TYPE.Industrial2of5:
                case TYPE.Standard2of5: //Encode_Standard2of5();
                    ibarcode = new Standard2of5(Raw_Data);
                    break;
                case TYPE.LOGMARS:
                case TYPE.CODE39: //Encode_Code39();
                    ibarcode = new Code39(Raw_Data);
                    break;
                case TYPE.CODE39Extended:
                    ibarcode = new Code39(Raw_Data, true);
                    break;
                case TYPE.Codabar: //Encode_Codabar();
                    ibarcode = new Codabar(Raw_Data);
                    break;
                case TYPE.PostNet: //Encode_PostNet();
                    ibarcode = new Postnet(Raw_Data);
                    break;
                case TYPE.ISBN:
                case TYPE.BOOKLAND: //Encode_ISBN_Bookland();
                    ibarcode = new ISBN(Raw_Data);
                    break;
                case TYPE.JAN13: //Encode_JAN13();
                    ibarcode = new JAN13(Raw_Data);
                    break;
                case TYPE.UPC_SUPPLEMENTAL_2DIGIT: //Encode_UPCSupplemental_2();
                    ibarcode = new UPCSupplement2(Raw_Data);
                    break;
                case TYPE.MSI_Mod10:
                case TYPE.MSI_2Mod10:
                case TYPE.MSI_Mod11:
                case TYPE.MSI_Mod11_Mod10:
                case TYPE.Modified_Plessey: //Encode_MSI();
                    ibarcode = new MSI(Raw_Data, Encoded_Type);
                    break;
                case TYPE.UPC_SUPPLEMENTAL_5DIGIT: //Encode_UPCSupplemental_5();
                    ibarcode = new UPCSupplement5(Raw_Data);
                    break;
                case TYPE.UPCE: //Encode_UPCE();
                    ibarcode = new UPCE(Raw_Data);
                    break;
                case TYPE.EAN8: //Encode_EAN8();
                    ibarcode = new EAN8(Raw_Data);
                    break;
                case TYPE.USD8:
                case TYPE.CODE11: //Encode_Code11();
                    ibarcode = new Code11(Raw_Data);
                    break;
                case TYPE.CODE128: //Encode_Code128();
                    ibarcode = new Code128(Raw_Data);
                    break;
                case TYPE.CODE128A:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.A);
                    break;
                case TYPE.CODE128B:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.B);
                    break;
                case TYPE.CODE128C:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.C);
                    break;
                case TYPE.ITF14:
                    ibarcode = new ITF14(Raw_Data);
                    break;
                case TYPE.CODE93:
                    ibarcode = new Code93(Raw_Data);
                    break;
                default:
                    throw new Exception("EENCODE-2: Unsupported encoding type specified.");
            }

            Encoded_Value = ibarcode.Encoded_Value;
            Raw_Data = ibarcode.RawData;
            Formatted_Data = ibarcode.FormattedData;
            _EncodingTime = (DateTime.Now - dtStartTime).TotalMilliseconds;

            EncodedImage = Generate_Image();

            _XML = GetXML();

            return EncodedImage;
        }

        #endregion

        #region Image Functions

        /// <summary>
        ///     Gets a bitmap representation of the encoded data.
        /// </summary>
        /// <returns>Bitmap of encoded value.</returns>
        private Bitmap Generate_Image()
        {
            if (string.IsNullOrEmpty(Encoded_Value)) throw new Exception("EGENERATE_IMAGE-1: Must be encoded first.");
            Bitmap b = null;

            switch (Encoded_Type)
            {
                case TYPE.ITF14:
                {
                    b = new Bitmap(Width, Height);

                    var bearerwidth = (int) (b.Width/12.05);
                    var iquietzone = Convert.ToInt32(b.Width*0.05);
                    var iBarWidth = (b.Width - bearerwidth*2 - iquietzone*2)/Encoded_Value.Length;
                    var shiftAdjustment = (b.Width - bearerwidth*2 - iquietzone*2)%Encoded_Value.Length/2;

                    if (iBarWidth <= 0 || iquietzone <= 0)
                        throw new Exception(
                            "EGENERATE_IMAGE-3: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel or quiet zone determined to be less than 1 pixel)");

                    //draw image
                    var pos = 0;

                    using (var g = Graphics.FromImage(b))
                    {
                        //fill background
                        g.Clear(BackColor);

                        //lines are fBarWidth wide so draw the appropriate color line vertically
                        using (var pen = new Pen(ForeColor, iBarWidth))
                        {
                            pen.Alignment = PenAlignment.Right;

                            while (pos < Encoded_Value.Length)
                            {
                                //lines are 2px wide so draw the appropriate color line vertically
                                if (Encoded_Value[pos] == '1')
                                    g.DrawLine(pen,
                                        new Point(pos*iBarWidth + shiftAdjustment + bearerwidth + iquietzone, 0),
                                        new Point(pos*iBarWidth + shiftAdjustment + bearerwidth + iquietzone, Height));

                                pos++;
                            }

                            //bearer bars
                            pen.Width = (float) b.Height/8;
                            pen.Color = ForeColor;
                            pen.Alignment = PenAlignment.Inset;
                            g.DrawLine(pen, new Point(0, 0), new Point(b.Width, 0)); //top
                            g.DrawLine(pen, new Point(0, b.Height), new Point(b.Width, b.Height)); //bottom
                            g.DrawLine(pen, new Point(0, 0), new Point(0, b.Height)); //left
                            g.DrawLine(pen, new Point(b.Width, 0), new Point(b.Width, b.Height)); //right
                        }
                    }

                    if (IncludeLabel)
                        Label_ITF14(b);

                    break;
                }
                case TYPE.PostNet:
                {
                    b = new Bitmap(Encoded_Value.Length*4, 20);

                    //draw image
                    for (var y = b.Height - 1; y > 0; y--)
                    {
                        var x = 0;
                        if (y < b.Height/2)
                        {
                            //top
                            while (x < b.Width)
                            {
                                if (Encoded_Value[x/4] == '1')
                                {
                                    //draw bar
                                    b.SetPixel(x, y, ForeColor);
                                    b.SetPixel(x + 1, y, ForeColor);
                                    b.SetPixel(x + 2, y, BackColor);
                                    b.SetPixel(x + 3, y, BackColor);
                                }
                                else
                                {
                                    //draw space
                                    b.SetPixel(x, y, BackColor);
                                    b.SetPixel(x + 1, y, BackColor);
                                    b.SetPixel(x + 2, y, BackColor);
                                    b.SetPixel(x + 3, y, BackColor);
                                }
                                x += 4;
                            }
                        }
                        else
                        {
                            //bottom
                            while (x < b.Width)
                            {
                                b.SetPixel(x, y, ForeColor);
                                b.SetPixel(x + 1, y, ForeColor);
                                b.SetPixel(x + 2, y, BackColor);
                                b.SetPixel(x + 3, y, BackColor);
                                x += 4;
                            }
                        }
                    }

                    break;
                }
                default:
                {
                    b = new Bitmap(Width, Height);

                    var iBarWidth = Width/Encoded_Value.Length;
                    var shiftAdjustment = Width%Encoded_Value.Length/2;

                    if (iBarWidth <= 0)
                        throw new Exception(
                            "EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                    //draw image
                    var pos = 0;

                    using (var g = Graphics.FromImage(b))
                    {
                        //clears the image and colors the entire background
                        g.Clear(BackColor);

                        //lines are fBarWidth wide so draw the appropriate color line vertically
                        using (var pen = new Pen(ForeColor, iBarWidth))
                        {
                            pen.Alignment = PenAlignment.Right;

                            while (pos < Encoded_Value.Length)
                            {
                                if (Encoded_Value[pos] == '1')
                                    g.DrawLine(pen, new Point(pos*iBarWidth + shiftAdjustment, 0),
                                        new Point(pos*iBarWidth + shiftAdjustment, Height));

                                pos++;
                            }
                        }
                    }

                    if (IncludeLabel)
                        Label_Generic(b);

                    break;
                }
            }

            EncodedImage = b;

            return b;
        }

        /// <summary>
        ///     Gets the bytes that represent the image.
        /// </summary>
        /// <param name="savetype">File type to put the data in before returning the bytes.</param>
        /// <returns>Bytes representing the encoded image.</returns>
        public byte[] GetImageData(SaveTypes savetype)
        {
            byte[] imageData = null;

            try
            {
                if (EncodedImage != null)
                {
                    //Save the image to a memory stream so that we can get a byte array!      
                    using (var ms = new MemoryStream())
                    {
                        SaveImage(ms, savetype);
                        imageData = ms.ToArray();
                        ms.Flush();
                        ms.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EGETIMAGEDATA-1: Could not retrieve image data. " + ex.Message);
            }
            return imageData;
        }

        /// <summary>
        ///     Saves an encoded image to a specified file and type.
        /// </summary>
        /// <param name="Filename">Filename to save to.</param>
        /// <param name="FileType">Format to use.</param>
        public void SaveImage(string Filename, SaveTypes FileType)
        {
            try
            {
                if (EncodedImage != null)
                {
                    ImageFormat imageformat;
                    switch (FileType)
                    {
                        case SaveTypes.BMP:
                            imageformat = ImageFormat.Bmp;
                            break;
                        case SaveTypes.GIF:
                            imageformat = ImageFormat.Gif;
                            break;
                        case SaveTypes.JPG:
                            imageformat = ImageFormat.Jpeg;
                            break;
                        case SaveTypes.PNG:
                            imageformat = ImageFormat.Png;
                            break;
                        case SaveTypes.TIFF:
                            imageformat = ImageFormat.Tiff;
                            break;
                        default:
                            imageformat = ImageFormat;
                            break;
                    }
                    ((Bitmap) EncodedImage).Save(Filename, imageformat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESAVEIMAGE-1: Could not save image.\n\n=======================\n\n" + ex.Message);
            }
        }

        /// <summary>
        ///     Saves an encoded image to a specified stream.
        /// </summary>
        /// <param name="stream">Stream to write image to.</param>
        /// <param name="FileType">Format to use.</param>
        public void SaveImage(Stream stream, SaveTypes FileType)
        {
            try
            {
                if (EncodedImage != null)
                {
                    ImageFormat imageformat;
                    switch (FileType)
                    {
                        case SaveTypes.BMP:
                            imageformat = ImageFormat.Bmp;
                            break;
                        case SaveTypes.GIF:
                            imageformat = ImageFormat.Gif;
                            break;
                        case SaveTypes.JPG:
                            imageformat = ImageFormat.Jpeg;
                            break;
                        case SaveTypes.PNG:
                            imageformat = ImageFormat.Png;
                            break;
                        case SaveTypes.TIFF:
                            imageformat = ImageFormat.Tiff;
                            break;
                        default:
                            imageformat = ImageFormat;
                            break;
                    }
                    ((Bitmap) EncodedImage).Save(stream, imageformat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESAVEIMAGE-2: Could not save image.\n\n=======================\n\n" + ex.Message);
            }
        }

        #endregion

        #region Label Generation

        private Image Label_ITF14(Image img)
        {
            try
            {
                var font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);

                using (var g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, 0, (float) 0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    //color a white box at the bottom of the barcode to hold the string of data
                    g.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, img.Height - 16, img.Width, 16));

                    //draw datastring under the barcode image
                    var f = new StringFormat {Alignment = StringAlignment.Center};
                    g.DrawString(Raw_Data, font, new SolidBrush(ForeColor), img.Width/2, img.Height - 16, f);

                    var pen = new Pen(ForeColor, (float) img.Height/16) {Alignment = PenAlignment.Inset};
                    g.DrawLine(pen, new Point(0, img.Height - 20), new Point(img.Width, img.Height - 20)); //bottom

                    g.Save();
                }
                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_ITF14-1: " + ex.Message);
            }
        }

        private Image Label_Generic(Image img)
        {
            try
            {
                var font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);

                using (var g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, 0, (float) 0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    //color a background color box at the bottom of the barcode to hold the string of data
                    g.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, img.Height - 16, img.Width, 16));

                    //draw datastring under the barcode image
                    var f = new StringFormat {Alignment = StringAlignment.Center};

                    var strLabelText = !string.IsNullOrEmpty(FormattedData) ? FormattedData : RawData;

                    g.DrawString(strLabelText, font, new SolidBrush(ForeColor), img.Width/2, img.Height - 16, f);

                    g.Save();
                }
                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_GENERIC-1: " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region Misc

        internal static bool CheckNumericOnly(string Data)
        {
            //This function takes a string of data and breaks it into parts and trys to do Int64.TryParse
            //This will verify that only numeric data is contained in the string passed in.  The complexity below
            //was done to ensure that the minimum number of interations and checks could be performed.

            //9223372036854775808 is the largest number a 64bit number(signed) can hold so ... make sure its less than that by one place
            var STRING_LENGTHS = 18;

            var temp = Data;
            var strings = new string[Data.Length/STRING_LENGTHS + (Data.Length%STRING_LENGTHS == 0 ? 0 : 1)];

            var i = 0;
            while (i < strings.Length)
                if (temp.Length >= STRING_LENGTHS)
                {
                    strings[i++] = temp.Substring(0, STRING_LENGTHS);
                    temp = temp.Substring(STRING_LENGTHS);
                }
                else
                    strings[i++] = temp.Substring(0);

            foreach (var s in strings)
            {
                long value = 0;
                if (!long.TryParse(s, out value))
                    return false;
            }

            return true;
        }

        private string GetXML()
        {
            if (string.IsNullOrEmpty(EncodedValue))
                throw new Exception(
                    "EGETXML-1: Could not retrieve XML due to the barcode not being encoded first.  Please call Encode first.");
            try
            {
                using (var xml = new BarcodeXML())
                {
                    var row = xml.Barcode.NewBarcodeRow();
                    row.Type = EncodedType.ToString();
                    row.RawData = RawData;
                    row.EncodedValue = EncodedValue;
                    row.EncodingTime = EncodingTime;
                    row.IncludeLabel = IncludeLabel;
                    row.Forecolor = ColorTranslator.ToHtml(ForeColor);
                    row.Backcolor = ColorTranslator.ToHtml(BackColor);
                    row.CountryAssigningManufacturingCode = Country_Assigning_Manufacturer_Code;
                    row.ImageWidth = Width;
                    row.ImageHeight = Height;

                    //get image in base 64
                    using (var ms = new MemoryStream())
                    {
                        EncodedImage.Save(ms, ImageFormat);
                        row.Image = Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.None);
                    }

                    xml.Barcode.AddBarcodeRow(row);

                    var sw = new StringWriter();
                    xml.WriteXml(sw, XmlWriteMode.WriteSchema);
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EGETXML-2: " + ex.Message);
            }
        }

        public static Image GetImageFromXML(BarcodeXML internalXML)
        {
            try
            {
                //converting the base64 string to byte array
                var imageContent = new byte[internalXML.Barcode[0].Image.Length];

                //loading it to memory stream and then to image object
                using (var ms = new MemoryStream(Convert.FromBase64String(internalXML.Barcode[0].Image)))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EGETIMAGEFROMXML-1: " + ex.Message);
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="Data">Raw data to encode.</param>
        /// <returns>Image representing the barcode.</returns>
        public static Image DoEncode(TYPE iType, string Data)
        {
            using (var b = new Barcode())
            {
                return b.Encode(iType, Data);
            }
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="Data">Raw data to encode.</param>
        /// <param name="XML">XML representation of the data and the image of the barcode.</param>
        /// <returns>Image representing the barcode.</returns>
        public static Image DoEncode(TYPE iType, string Data, ref string XML)
        {
            using (var b = new Barcode())
            {
                var i = b.Encode(iType, Data);
                XML = b.XML;
                return i;
            }
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="Data">Raw data to encode.</param>
        /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <returns>Image representing the barcode.</returns>
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel)
        {
            using (var b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data);
            }
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="data">Raw data to encode.</param>
        /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <param name="Width">Width of the resulting barcode.(pixels)</param>
        /// <param name="Height">Height of the resulting barcode.(pixels)</param>
        /// <returns>Image representing the barcode.</returns>
        public static Image DoEncode(TYPE iType, string data, bool IncludeLabel, int Width, int Height)
        {
            using (var b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, data, Width, Height);
            }
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="Data">Raw data to encode.</param>
        /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <param name="DrawColor">Foreground color</param>
        /// <param name="BackColor">Background color</param>
        /// <returns>Image representing the barcode.</returns>
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor)
        {
            using (var b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data, DrawColor, BackColor);
            }
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="Data">Raw data to encode.</param>
        /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <param name="DrawColor">Foreground color</param>
        /// <param name="BackColor">Background color</param>
        /// <param name="Width">Width of the resulting barcode.(pixels)</param>
        /// <param name="Height">Height of the resulting barcode.(pixels)</param>
        /// <returns>Image representing the barcode.</returns>
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor,
            int Width, int Height)
        {
            using (var b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data, DrawColor, BackColor, Width, Height);
            }
        }

        /// <summary>
        ///     Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="Data">Raw data to encode.</param>
        /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <param name="DrawColor">Foreground color</param>
        /// <param name="BackColor">Background color</param>
        /// <param name="Width">Width of the resulting barcode.(pixels)</param>
        /// <param name="Height">Height of the resulting barcode.(pixels)</param>
        /// <param name="XML">XML representation of the data and the image of the barcode.</param>
        /// <returns>Image representing the barcode.</returns>
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor,
            int Width, int Height, ref string XML)
        {
            using (var b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                var i = b.Encode(iType, Data, DrawColor, BackColor, Width, Height);
                XML = b.XML;
                return i;
            }
        }

        #endregion
    }
}