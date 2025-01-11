using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHome.Models.Locale
{
    public class Naming
    {
        private Naming()
        {

        }

        public enum DataResultMode
        {
            Display = 0,
            Print = 1,
            Download = 2
        }

        public enum DocumentTypeDefinition
        {
            Professional = 1,
            Knowledge = 2,
            Rental = 3,
            Products = 4,
            Cooperation = 5,
            ContactUs = 6,
            Inspirational = 7
        }

        public enum DeviceLevelDefinition
        {
            已刪除 = 0,
            正常 = 1,
            斷線 = 2,
            保全解除 = 3,
            保全設定 = 4,
            火災 = 5,
            瓦斯異常 = 6,
            緊急 = 7,
            反脅迫警告 = 8,
            迴路一異常 = 9,
            迴路二異常 = 10,
            迴路三異常 = 11,
            迴路四異常 = 12,
            迴路五異常 = 13,
        }

        public static String[] DeviceStatusCode = { "", "00", "99", "R", "S", "F", "GS", "C", "TD", "T1", "T2", "T3", "T4", "T5" };

        public enum DefenceStatus
        {
            Clear = -1,
            Secured = 1,
        }

        public enum MemberStatusDefinition
        {
            ReadyToRegister = 1001,
            Deleted = 1002,
            Checked = 1003,
            Anonymous = 1004 
        }

        public enum RoleID
        {
            Administrator = 1,
            Coach = 2,
            FreeAgent = 3,
            Learner = 4,
            Guest = 5,
            Accounting = 6,
            Officer = 7,
            Assistant = 8
        }

        public enum LessonStatus
        {
            準備上課 = 100,
            上課中 = 101,
            課程結束 = 102
        }

        public enum QuestionType
        {
            問答題 = 200,
            單選題 = 201,
            多重選 = 202,
            是非題 = 203,
            單選其他 = 204,
            多重選其他 = 205
        }

        public enum FitnessAssessmentGroup
        {
            檢測體能 = 1,
        }

        public enum IncommingMessageStatus
        {
            未讀 = 3,
            已讀 = 4
        }

        public enum AlarmMode
        {
            release = 0,
            alarm = 1
        }

        public enum AlarmSubscription
        {
            公共設施 = -1,
            迴路一 = 0,
            迴路二 = 1,
            迴路三 = 2,
            迴路四 = 3,
            迴路五 = 4,
            迴路六 = 5,
            迴路七 = 6,
            迴路八 = 7,
        }

        public enum SensorType
        {
            火災 = 0,
            瓦斯,
            紅外,
            門磁,
            窗磁,
            緊急按鈕,
            浸水,
            緊急繩索,
            床頭按鈕,
        }

        public enum CommunicationMode
        {
            All = 0,
            中保 = 1,
            新保 = 2,
            AwtekOnly = 3,
            ControlCenter = 4,
        }

    }
}