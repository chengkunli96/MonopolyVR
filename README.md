# MonopolyVR

![Open Source Love](https://badges.frapsoft.com/os/v1/open-source.svg?v=103)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)
[![GitHub stars](https://img.shields.io/github/stars/chengkunli96/MonopolyVR.svg?style=social)](https://github.com/chengkunli96/MonopolyVR/stargazers)

MonopolyVR is a VR game based on an American classic board game, Monopoly. You can buy the land and build houses, and you may also trigger some random events such as a puzzle game. 

For a better gaming experience of users, we add more elements for  collaboration part. And we set a final goal for this game. The purpose of this game is to earn more money to build a house together in the center of the map. In order to achieve the goal, participants need to communicate with others to complete a puzzle game together and buy materials to establish their building. 

## Dependencies

We build our VR game on windows10 64-bit. For other dependencies:

* Unity 2019.4.10f1

* Ubik-0.0.4 (Used for our network part. And if you want to use it, please contact [UCL VECG lab](http://vecg.cs.ucl.ac.uk/) )

For installation:

* In `Unity` folder, you will get 3 folders: `Assets`, `Packages`, and `ProjectSettings`.
* Create a new project, then paste these three folders under the new project folder and replace the `Assets`, `Packages`, and `Projectsettings` automatically created by the new project.
* Click the unity scene named `GameMap` (`Assets->Samples->Ubik->0.0.4->Samples->GameMap`) and you will find some errors in the player (missing prefab). In order to fix this, click `window->package manager->add package from disk` to import `ubik-0.0.4's package.json` file and the errors will be fixed.

## Samples

You can see more details in `Report`floder,  including our report and video sample. 

And the over view of this game is following:

![over view](https://github.com/mremilien/MonopolyVR/blob/master/images/over_view.png)

## Acknowledgements

This project would not have been possible without the help of my teammates, Zijian Guo, Jiamin Wang and Zhaoyi Tan.


  

  

  
