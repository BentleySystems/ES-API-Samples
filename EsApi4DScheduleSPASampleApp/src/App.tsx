import React, { useCallback, useEffect, useState } from "react";
import Axios from "axios";
import { BrowserAuthorizationClient } from "@itwin/browser-authorization";
import './App.css';
import jwt_decode from "jwt-decode";

const App: React.FC = () => {

  const [windowUrl, setWindowUrl] = useState(new URL(window.location.href));
  const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);
  const [userClientId, setClientId] = useState('');
  const [scheduleId, setScheduleId] = useState('');
  const [getResponse, setGetResponse] = useState({"items": []});
  const [postResponse, setPostResponse] = useState({"changeRequestId": ""});
  const [changeResponse, setChangeResponse] = useState({"id": "", "status": "", "failureDescription": ""});
  const [token, setToken] = useState<string>("");

  useEffect(() => {
    handle(windowUrl);
  }, [windowUrl]);

  const url = "https://es-api.bentley.com/4dschedule/v1/schedules/" + scheduleId + "/resourceStatusHistory";

  const authClient =
      new BrowserAuthorizationClient({
        clientId: userClientId,
        scope: "enterprise offline_access",
        redirectUri: "https://localhost:3000/signin-callback",
        postSignoutRedirectUri: "https://localhost:3000/logout",
        responseType: "code",
        silentRedirectUri: "https://localhost:3000/signin-callback",
        authority: "https://ims.bentley.com/"
      });


  const login = useCallback(async () => {
    try {
      await authClient.signInPopup();
      setIsLoggedIn(authClient.isAuthorized);
      const tok = await authClient.getAccessToken();
      setToken(tok);
    } catch (error : any) {
      alert(error);
      console.log("error in login: " + error);
    }
  }, [authClient]);

  const logout = useCallback(async () => {
    try {
      await authClient.signOutRedirect();
    } catch {
      await authClient.signOut();
    }
  }, [authClient]);

  const obtainNewToken = async () => {
    await authClient.signInSilent();
    await authClient.handleSigninCallback();
    const tok = await authClient.getAccessToken();
    setToken(tok);
    return tok;
  }

  const handle = async (url: URL) => {
    if (url.pathname === '/signin-callback') {
      try {
        await authClient.handleSigninCallback();
      }
      catch ( err : any ) {
        alert("error in handle: " + err);
      }
    }
  };

  const getRequest = async () => {
    const decodedToken = await jwt_decode(token) as any;
    if (Date.now() >= decodedToken.exp * 1000) {
      const newTok = await obtainNewToken();
      getFunction(newTok);
    } else {
      getFunction(token);
    }
  }

  const getFunction = (tok) => {
    var config = {
      headers: { Authorization: tok }
    }
    try {
      Axios.get(url, config).then((axiosResponse)=>{setGetResponse(axiosResponse.data)}).catch(
        function (error) {
          console.log(error);
          alert("Error in GET: " + error.message);
        }
      );
    } catch (err: any) {
      alert("Error in GET: " + err);
    }
  }

  const postRequest = async () => {
    const decodedToken = jwt_decode(token) as any;
    if (Date.now() >= decodedToken.exp * 1000) {
      const newTok = await obtainNewToken();
      postFunction(newTok);
    } else {
      postFunction(token);
    }
  }

  const postFunction = (tok) => {
    var config = {
      headers: { Authorization: tok }
    }
    try {
      const body = {
        "changeRequestId": crypto.randomUUID,
        "item": {
          "resourceId": getResponse.items[0].resourceId,
          "date": "2023-07-06T14:20:57.774Z",
          "statusCategoryId": getResponse.items[0].statusCategoryId,
          "statusItemId": getResponse.items[0].statusItemId
        }
      }
      Axios.post(url, body, config).then((axiosResponse)=>{setPostResponse(axiosResponse.data)}).catch(
        function (error) {
          alert("Error in POST: " + error.message);
        }
      );
    } catch (err: any) {
      alert("Error in POST: " + err);
    }
  }

  const changeRequest = async () => {
    const decodedToken = jwt_decode(token) as any;
    if (Date.now() >= decodedToken.exp * 1000) {
      const newTok = await obtainNewToken();
      changeFunction(newTok);
    } else {
      changeFunction(token);
    }
  }

  const changeFunction = (tok) => {
    var config = {
      headers: { Authorization: tok }
    }
    try {
      const changeRequestUrl = "https://es-api.bentley.com/4dschedule/v1/schedules/" + scheduleId + "/changeRequests/" + postResponse.changeRequestId;
      Axios.get(changeRequestUrl, config).then((axiosResponse)=>{setChangeResponse(axiosResponse.data)}).catch(
        function (error) {
          alert("Error in changeRequests GET: " + error.message);
        }
      );
    } catch (err: any) {
      alert("Error in changeRequests GET: " + err);
    }
  }

  const setSchedule = (event) => {
    setScheduleId(event.target.value);
  }

  const setId = (event) => {
    setClientId(event.target.value);
  }

  const renderLogin = () => {
    return (
      <>
          <h3>Logged in at {userClientId}</h3>
          <button onClick={logout}>Logout</button>
          <p>Set your schedule ID</p>
          <input id="scheduleId" onChange={setSchedule} value={scheduleId} />
          <br />
          <h4>Send GET to resourceStatusHistory endpoint</h4>
          <button disabled={scheduleId.length<1} onClick={getRequest}>GET</button>
          <br /><br />
          <h5>GET Response:</h5>
          <textarea readOnly value={JSON.stringify(getResponse)} />
          <h4>Send POST to resourceStatusHistory endpoint</h4>
          <button disabled={scheduleId.length<1 || getResponse.items.length<1} onClick={postRequest}>POST</button>
          <br /><br />
          <h5>POST Response:</h5>
          <textarea readOnly value={JSON.stringify(postResponse)} />
          <h4>Send GET to changeRequests endpoint</h4>
          <button disabled={scheduleId.length<1 || postResponse.changeRequestId === ""} onClick={changeRequest}>GET</button>
          <br /><br />
          <h5>GET Response:</h5>
          <textarea readOnly value={JSON.stringify(changeResponse)} />
      </>
    )
  }

  const renderLogOut = () => {
    return (
      <>
        <h3>Logged out</h3>
        <p>Enter your client ID</p>
        <input id="clientId" onChange={setId} value={userClientId} />
        <br />
        <button disabled={userClientId.length<1} onClick={login}>Login</button>
        
      </>
    )
  }

  return (
    <>
      <h2>Sample SPA App interfacing with 4D Schedules External API</h2>
      {<p>{token}</p>}
      {isLoggedIn ? renderLogin() : renderLogOut()}
      
    </>
  );
};

export default App;
