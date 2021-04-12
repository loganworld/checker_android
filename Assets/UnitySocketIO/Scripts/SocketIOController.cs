using UnityEngine;
using System;
using System.Collections;
using UnitySocketIO.SocketIO;
using UnitySocketIO.Events;

namespace UnitySocketIO {
    public class SocketIOController : MonoBehaviour {
        
        public SocketIOSettings settings;
        public string domain = "localhost";
        public  BaseSocketIO socketIO;
        public static SocketIOController instance;
        // public bool isTesting = false;
        public string SocketID { get { return socketIO.SocketID; } }

        void Awake() {

            if (instance != null)
                Destroy(instance.gameObject);
            instance = this;
            
            DontDestroyOnLoad(gameObject);

            if(Application.platform == RuntimePlatform.WebGLPlayer) {
                socketIO = gameObject.AddComponent<WebGLSocketIO>();
            }
            else {
                socketIO = gameObject.AddComponent<NativeSocketIO>();
            }

            //if (Application.platform == RuntimePlatform.WindowsEditor)
            //{
            //    settings.url = "localhost";//Global.DOMAIN;
            //}
            //else
            //{
                settings.sslEnabled = Global.SSL_ENALBLED;
                settings.url = Global.DOMAIN;// Global.DOMAIN;
                settings.port = Global.PORT;
            //}

            

            if (Global.isTesting)
            {
                settings.sslEnabled = false;
                settings.url = Global.testingURL;
                settings.port = Global.testingPORT;
                Debug.Log("Testing...");
            }
            
            Debug.Log("Port : " + settings.port);
            Debug.Log("URL : " + settings.url);

            socketIO.Init(settings);
        }

        private void Start()
        {

            Global.socketConnected = false;

            On("connected", Connected);


            StartCoroutine(iReconnect());
        }

        IEnumerator iReconnect()
        {

            yield return new WaitForSeconds(0.5f);

            if (Global.socketConnected)
            {
                yield break;

            }

            Debug.Log("*******Socket Connecting...");
            Connect();
            // StartCoroutine(iReconnect());
        }
        private void Connected(SocketIOEvent obj)
        {
            Debug.Log("Socket Connected.");
            Global.socketConnected = true;
        }

        public void Connect() {
            socketIO.Connect();
        }

        public void Close() {
            socketIO.Close();
        }

        public void Emit(string e) {
            socketIO.Emit(e);
        }
        public void Emit(string e, Action<string> action) {
            socketIO.Emit(e, action);
        }
        public void Emit(string e, string data) {
            socketIO.Emit(e, data);
        }
        public void Emit(string e, string data, Action<string> action) {
            socketIO.Emit(e, data, action);
        }

        public void On(string e, Action<SocketIOEvent> callback) {
            socketIO.On(e, callback);
        }
        public void Off(string e, Action<SocketIOEvent> callback) {
            socketIO.Off(e, callback);
        }



    }
}