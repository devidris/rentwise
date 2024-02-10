importScripts('https://www.gstatic.com/firebasejs/9.14.0/firebase-app-compat.js')
importScripts('https://www.gstatic.com/firebasejs/9.14.0/firebase-messaging-compat.js')


const firebaseConfig = {
    apiKey: "AIzaSyBaSMKCA0Toe3O_4LFHUxgEEDhf_-avHwY",
    authDomain: "rent-wise-dc09f.firebaseapp.com",
    projectId: "rent-wise-dc09f",
    storageBucket: "rent-wise-dc09f.appspot.com",
    messagingSenderId: "342704319982",
    appId: "1:342704319982:web:c238396bf5ebac8aa6b2ff",
    measurementId: "G-QYWYSJ5BTE"
};
const app = firebase.initializeApp(firebaseConfig)
const messaging = firebase.messaging()