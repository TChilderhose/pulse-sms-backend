# pulse-sms-backend

When Pulse SMS was bought by Maple Media (https://www.androidpolice.com/2020/10/29/it-looks-like-pulse-sms-has-been-bought-by-maple-media-get-ready-for-intrusive-ads/) I started working on a backend that I could selfhost and just fork the android client, manually swap the urls and keys and use that.

Eventually Maple Media make the repo private so I stopped working on it because I don't have time to do upkeep on the android side of stuff. So I figured I would dump my findings and some initial code that I was working on.


Background
- It's written in NetCore 3.1 (probably should switched to .Net5 now that it's out) uses EntityFrameworkCore since those are things I use in my day job
- Right now it just creats an SQLite database because it's quick to nuke and start fresh. I had the intention to switch to a more robust relational database once rapid development was done.
- The initial idea was in the end to build a docker image and pass in some environment variables to get working

Findings
- `pulse-sms-android`
  - There is inconsistencies with naming and case. Sometimes it expects camelcase, sometimes it expect snake case
  - It does seem to be E2EE, but the `api/v1/accounts/signup` and `api/v1/accounts/login` are sending the password in plain text... So in theory they could be storing that password when you login, and then they have everything they need (password and salt2) to decrypt the messages. My recommendation would be to first hash the password on the client when sending the signup and login requests (continue to hash it on the server as well) and continue to use the raw password with `salt2` to generate the E2EE key. That way the server can't decrypt the messages no matter what.
  - It uses Firebase data messages to share data between clients. It works well, but also means it's reliant on Google services
  
Setup
- `pulse-sms-backend`
  - I would just comment out `Pulse.Helpers.FirebaseHelper` stuff for now. I was starting to work on that as I was interested in how it actually works or if you know how to setup a Firebase app set an environment variable `FIREBASE_SERVER_KEY`
  - Otherwise it should be working if 
- `pulse-sms-android`
  - Most likely you'll be starting out without any certificate on your webserver, so android will need some config to allow that.
    Add the file `pulse-sms-android\app\src\main\res\xml\security_config.xml` with
    ```
    <?xml version="1.0" encoding="utf-8"?>
    <network-security-config>
        <base-config cleartextTrafficPermitted="true">
            <trust-anchors>
                <certificates src="system" />
            </trust-anchors>
        </base-config>
    </network-security-config>
    ```
    and add `android:networkSecurityConfig="@xml/security_config"` to the `pulse-sms-android\app\src\main\AndroidManifest.xml` file in the `application` block (around line 83)
  - In `pulse-sms-android\api\src\main\java\xyz\klinker\messenger\api\Api.java` replace `API_DEBUG_URL` or `API_RELEASE_URL` with the url you are going to be using. I say `API_RELEASE_URL` because a bunch of code needs to be uncommented/changed to get the debug url to be used.
  - For Firebase, there are a lot of guides around, to set it up in the google console. But the idea is you set it up, grab the `google-services.json` it gives you and replace all of them in the app you can find. You will also need ot replace the `FIREBASE_STORAGE_URL` in `pulse-sms-android\api_implementation\src\main\java\xyz\klinker\messenger\api\implementation\ApiUtils.kt`
  - Go to `pulse-sms-android\api_implementation\src\main\java\xyz\klinker\messenger\api\implementation\LoginActivity.java` and in `setUpInitialLayout` comment out the if statement in there so that it doesn't disable signups (eventually I was going to strip all of the IAP code)

Current State
- I have it spitting out all the requests it receives in console, just incase I missed any
- I took all of the models and endpoints that were in the android app and convered them to the c# equivelent
- The relationships between everything should be close to being done (`Pulse.Models.PulseDbContext`)
- I got 90% of the api calls working, including signup and I believe logining working as well. Messages and new conversations should send as well.
- I basically would start using the app, see what requests come into the server and start working on the endpoints that I hadn't yet.
- I was starting to work on the multi-device support starting with the Firebase data messaging
- There was afew endpoints that I never hit so didn't do anything with them yet.
- Also should note, some of the code is in a messy state, sorry about that. Once it was up an running I was going to do my cleanup pass.
