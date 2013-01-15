using System;
using System.Collections.Generic;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// ��Ϣ��ʾ
    /// </summary>
    /// <param name="message"></param>
    public delegate void ShowMessageHandler(string message);
    /// <summary>
    /// ����״̬�����仯 ÿ��UpdateService ��״̬�ı仯 ��ִ��ǰ��ִ���У�ִ�к�ִ�гɹ���ִ��ʧ��
    /// </summary>
    /// <param name="updateServiceService"></param>
    public delegate void StateChangeHandler(IUpdateService updateServiceService);
    /// <summary>
    ///����״̬�����仯   ÿ��updateSercie �ı仯����updateService �ɹ���ʧ�ܣ�����ִ���� ��Ҫ���ڽ���������ʾ
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    //public delegate void ProcessStateChangeHandler(string message, StateChangeSource source);
    /// <summary>
    /// ��������
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// ��Ϣ��ʾ�¼�
        /// </summary>
        event ShowMessageHandler ShowMessage;
        /// <summary>
        /// ״̬�仯�����¼�
        /// </summary>
        event StateChangeHandler StateChange;
        /// <summary>
        /// ��������
        /// </summary>
        string Title { get; }
        /// <summary>
        /// �������������ļ���ַ
        /// </summary>
        string UpdateFolderName { set; }
        /// <summary>
        /// Ŀ���ַ
        /// </summary>
        string TargetPath { set; }
        /// <summary>
        /// ��ʼִ������
        /// </summary>
        void Update();
        /// <summary>
        /// ״̬
        /// </summary>
        UpdateState State { get; set; }
        /// <summary>
        /// �����ļ��е�ַ
        /// </summary>
        string BackUpFoler { get; set; }

    }
}
