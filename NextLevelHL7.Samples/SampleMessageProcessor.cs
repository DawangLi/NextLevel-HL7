using System;
using System.Collections.Generic;
using NextLevelHL7.Extensions;

namespace NextLevelHL7.Samples
{
    public class SampleMessageProcessor
    {
        /// <summary>
        /// Demonstrates how to read an HL7 ADT ("Admit Dischrage Transfer") message.  
        /// </summary>
        /// <param name="message">The HL7 message.</param>
        public void HandleADT(Message message)
        {
            // message type
            string messageType = message.MessageType();

            // retrieve patient identifier
            Segment pid = message.FindSegment("PID");

            // retrieve other message types
            Segment evn = message.FindSegment("EVN");
            Segment pv1 = message.FindSegment("PV1");
            Segment pv2 = message.FindSegment("PV2");
            Segment zlo = message.FindSegment("ZLO");

            // switch on message type to show admit, dischage, or transfer
            string sampleResult = string.Empty;
            switch (messageType)
            {
                case "ADT^A01":
                    sampleResult = "Patient {0} (DOB {1}) has been admitted to {2}.  Reason: {3}.  Date: {4:g}";
                    break;
                case "ADT^A02":
                    sampleResult = "Patient {0} (DOB {1}) has been transferred to {2}.  Reason: {3}.  Date: {4:g}";
                    break;
                case "ADT^A03":
                    sampleResult = "Patient {0} (DOB {1}) has been discharged from {2}.  Reason: {3}.  Date: {4:g}";
                    break;
                case "ADT^A04":
                    sampleResult = "Patient {0} (DOB {1}) has been admitted to {2}.  Reason: {3}.  Date: {4:g}";
                    break;
            }

            string patientName = pid[5][0] + " " + pid[5][1] + " " + pid[5][2];
            string dob = pid[7][0].Substring(4, 2) + "/" + pid[7][0].Substring(6, 2) + "/" + pid[7][0].Substring(0, 4);
            string location;
            if (zlo != null && !string.IsNullOrEmpty(zlo[2]))
                location = zlo[2];
            else
                location = pv1[3][0];
            string reason = pv2[3];
            if (reason != null)
                reason = reason.Trim('^');
            DateTime? date = evn[2].ToString().ParseDate();

            sampleResult = string.Format(sampleResult, patientName, dob, location, reason, date);
        }

        /// <summary>
        /// Demonstrates how to read an HL7 ORU ("Observation Result") message
        /// </summary>
        /// <param name="message">The HL7 message.</param>
        public void HandleORU(Message message)
        {
            // retrieve patient identifier
            Segment pid = message.FindSegment("PID");
            Segment segment = message.FindSegment("OBR");

            List<Segment> obxSegments = message.FindSegments("OBX");
            foreach (Segment obx in obxSegments)
            {
                // iterate through each result within a given obseration ("OBX")
                string accessionID = segment[20][0];
                string loinc = obx[3][0];
                DateTime? resultDate = obx[14].ToString().ParseDate();
                string resultStatus = null;
                if (obx[11])
                    resultStatus = obx[11];
                string resultType = obx[3][4];
                string resultUnits = obx[6][1];
                bool resultIsAbnormal = obx[8] == "Y";
                string resultReferenceRange = obx[7][0];
                float numericResult;
                float.TryParse(obx[5], out numericResult);
                string valueText = obx[5].ToString();
                if (!string.IsNullOrEmpty(valueText))
                    valueText = (valueText.Substring(0, Math.Min(valueText.Length, 512)));
            }
        }

        /// <summary>
        /// Demonstrates how to read an HL7 ADT Patient Merge message
        /// </summary>
        /// <param name="message">The message.</param>
        public void HandleMerge(Message message)
        {
            Segment pid = message.FindSegment("PID");
            Segment mrg = message.FindSegment("MRG");
            if (pid == null || mrg == null)
                return;

            string newMRN = pid[2][0];
            string oldMRN = mrg[1][0];

            // execute patient merge here using new and old MRN
        }
    }
}
