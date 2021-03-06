# LibraryScrobbler
Are you dedicated to keeping track of your listening habits, but sometimes find yourself listening through devices that don't support automatic scrobbling?

Perhaps you're frustrated by either short or obscure music tracks that always fail to automatically scrobble even when you're listening at home?

Or maybe you've simply just accidentally deleted a scrobble and noticed that there's no good way to bring it back. 
LibraryScrobbler exists to solve all these problems (and possibly more!) by granting you full, manual control over what you want scrobbled.

## The Issue(s)
Although convenient, automatic scrobbling through Last.fm occasionaly results in both doubled and skipped tracks. Because of this, it becomes necessary that you have the ability to both remove and add scrobbles in order to fix such issues.
Last.fm has the first one covered, making it easy to delete old scrobbles through the web, but surprisingly, it provides no good way to add new ones!

This is where external tools come in, giving you a means to fill in the gaps where automatic scrobbling has fallen short. 

Yet even among these tools, we encounter yet another issue: **dependence upon an external database**.

These sources, no matter how thorough, inevitably contain either incomplete or even conflicting information to what you've been automatically scrobbling yourself, which only makes the situation even more of a pain to manage than it was before!

## The Solution
Because you (and I) scrobble, first and foremost, to detail our own personal musical journies, there should exist a lightweight tool that caters to your own personal music catalogues.
After all, if everyone can't agree on what information is 'accurate', then it only makes sense that everyone is enabled to at least record their own version of what they feel should be.

LibraryScrobbler was created to facilitate this goal through two core modules:
1. Parse through ANY music library, and organize a collection of tags.
2. Query this tag collection, and present the results in a simple interface that makes manual scrobbling just as quick & easy as playing your music.

Step 1. Requires only an input and output path, and produces data in both JSON and SQLite forms.

Step 2. Requires the output from Step 1, plus some user information necessary to send your scrobbles to Last.fm.
This neccessary information includes both an API Key and a Secret in addition to your standard login, all of which is free to setup ([details here!](https://www.last.fm/api/account/create))
