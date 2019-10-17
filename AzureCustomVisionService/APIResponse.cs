using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureCustomVisionService
{
    public class VisionAPIResponse
    {
        public string id;
        public string project;
        public string iteration;
        public string created;
        public List<Prediction> predictions = new List<Prediction>();
    }

    public class Prediction
    {
        public double probability;
        public string tagName;
        public BoundingBox boundingBox = new BoundingBox();
    }

    public class BoundingBox
    {
        public double left;
        public double top;
        public double width;
        public double height;
    }

    public class RecognitionAPIResponse
    {
        public string status;
        public RecognitionResult recognitionResult = new RecognitionResult();
    }
    public class RecognitionResult
    {
        public List<Line> lines = new List<Line>();
    }

    public class Line
    {
        public string text;
    }

    public class State
    {
        public string sourceImagesFilePath;
        public string outputImageFilePath;
        public int workItemcount;
        public ManualResetEvent manualEvent;

        public State(string sourceFileName, string destinationFileName, int count, ManualResetEvent manualEvent)
        {
            this.sourceImagesFilePath = sourceFileName;
            this.outputImageFilePath = destinationFileName;
            this.workItemcount = count;
            this.manualEvent = manualEvent;
        }
    }
}
