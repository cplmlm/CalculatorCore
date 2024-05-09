using CalculatorCore.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CalculatorCore.ViewModel
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        #region 字段
        private string resultText = string.Empty;//计算结果
        private string first = string.Empty;//运算符前的值
        private string second = string.Empty;//运算符后的值
        private string operators = string.Empty;//运算符
        #endregion 
        #region 属性及绑定事件
        /// <summary>
        /// 输入文本框的值
        /// </summary>
        public string ResultText
        {
            get { return resultText; }
            set
            {
                resultText = value;
                RaisePropertyChanged("ResultText");
            }
        }
        /// <summary>
        /// 数字按钮绑定事件
        /// </summary>
        public ICommand NumberCommand
        {
            get { return new RelayCommand<string>(CanNumberContent, CanExecute); }
        }
        /// <summary>
        /// 运算符按钮绑定事件
        /// </summary>
        public ICommand OperatorsCommand
        {
            get { return new RelayCommand<string>(CanOperatorsContent, CanExecute); }
        }
        /// <summary>
        /// 计算结果按钮绑定事件
        /// </summary>
        public ICommand ResultCommand
        {
            get { return new QueryCommand(CanCalculationResult, CanExecute); }
        }
        /// <summary>
        /// 删除按钮绑定事件
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return new QueryCommand(CanDelete, CanExecute); }
        }
        /// <summary>
        /// 清空按钮绑定事件
        /// </summary>
        public ICommand CleanCommand
        {
            get { return new QueryCommand(CanClean, CanExecute); }
        }
        /// <summary>
        /// 百分比绑定事件
        /// </summary>
        public ICommand PercentCommand
        {
            get { return new QueryCommand(CanPercent, CanExecute); }
        }
        #endregion
        #region 绑定事件的执行
        /// <summary>
        /// 计算文本值
        /// </summary>
        /// <param name="buttonContent">按钮文本值</param>
        public void CanNumberContent(string buttonContent)
        {
            //没有点击运算符按钮就给运算符前赋值，点击了运算符按钮就给运算符后赋值
            if (string.IsNullOrWhiteSpace(operators))
            {
                //为0并且不包含小数点的时候为空
                string result = (first == "0" && !buttonContent.Contains(".") ? string.Empty : first);
                //限制已经输入了小数点就不能再输入
                string newValue = ResultText.Contains(".") && buttonContent.Contains(".") ? string.Empty : buttonContent;
                first = result + newValue;
                ResultText = first;
            }
            else
            {
                second = second + buttonContent;
                ResultText = first + operators + second;
            }
        }
        /// <summary>
        /// 运算符
        /// </summary>
        /// <param name="buttonContent">按钮文本值</param>
        public void CanOperatorsContent(string buttonContent)
        {
            //运算符后的值不为空就计算一下结果
            if (!string.IsNullOrWhiteSpace(second))
            {
                CanCalculationResult();
            }
            //获取最后一个字符判读是否是小数点，如果是就移除
            string endStr = ResultText.Substring(ResultText.Length - 1);
            if (endStr == ".")
            {
                first = ResultText.Remove(ResultText.Length - 1);
            }
            operators = buttonContent;
            ResultText = first + buttonContent;
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        public void CanCalculationResult()
        {
            double result = 0;
            double firstNumber = 0;
            double.TryParse(first, out firstNumber);
            double secondNumber = 0;
            double.TryParse(second, out secondNumber);
            if (!string.IsNullOrWhiteSpace(operators))
            {
                switch (operators)
                {
                    case "+":
                        result = firstNumber + secondNumber;
                        break;
                    case "-":
                        result = firstNumber - secondNumber;
                        break;
                    case "x":
                        result = firstNumber * secondNumber;
                        break;
                    case "÷":
                        result = secondNumber == 0 ? 0 : firstNumber / secondNumber;
                        break;
                }
                ResultText = result.ToString();
                //将计算结果赋给运算符前的值，并将运算符、运算符后的值清空
                first = ResultText;
                second = string.Empty;
                operators = string.Empty;
            }
        }
        /// <summary>
        /// 清空所有的值
        /// </summary>
        private void CanClean()
        {
            ResultText = string.Empty;
            operators = string.Empty;
            first = string.Empty;
            second = string.Empty;
        }
        /// <summary>
        /// 删除操作
        /// </summary>
        private void CanDelete()
        {
            //调用remove移除最后一个字符
            ResultText = ResultText.Length > 0 ? ResultText.Remove(ResultText.Length - 1) : string.Empty;
            //如果包含运算符就删除运算符后的值，不包含就删除运算符前的值
            if (ResultText.Contains("+") || ResultText.Contains("-") || ResultText.Contains("x") || ResultText.Contains("÷"))
            {
                second = second.Remove(second.Length - 1);
            }
            else
            {
                first = resultText;
                operators = string.Empty;
            }
        }
        /// <summary>
        /// 百分比
        /// </summary>
        private void CanPercent()
        {
            if (!string.IsNullOrEmpty(second))
            {
                //按百分比重新计算
                double result = 0;
                double.TryParse(second, out result);
                second = (result / 100).ToString();
                ResultText = first + operators + second;
            }
            else
            {
                ResultText = "0";
                first = string.Empty;
            }
        }
        public bool CanExecute()
        {
            return true;
        }
        public bool CanExecute(string content)
        {
            return true;
        }
        #endregion 

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion 
    }
}
