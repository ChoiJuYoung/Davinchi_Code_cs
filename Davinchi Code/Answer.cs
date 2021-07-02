using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davinchi_Code
{
    class Answer
    {
        string ans;
        public Answer(int MyNum, int PickPla, int AnswerClickIndex, string TheAnswerCo)
        {
            ans = Microsoft.VisualBasic.Interaction.InputBox("답을 입력하세요!");
            ans = "DC%assert%" + MyNum.ToString() + "%" + PickPla.ToString() + "%" + AnswerClickIndex.ToString() + "%" + TheAnswerCo + "%" + ans;
        }

        public string getAns()
        {
            return ans;
        }
    }
}
