# DeveloperFX app

Ceci est mon repo pour l'application qui fait le "bridge" entre le site web ainsi que l'application MetaTrader. 

L'application est codé en C#, utilise WPF pour le UI et ensuite un tcp server ainsi qu'un WebSocket server.

On commence avec un ui qui permet de lancer l'application MetaTrader avec les bonnes configurations, par la suite on tombe dans un process en background qui fait ceci : 

L'infrastructure est est un serveur socket TCP qui permet de communiquer avec l'application MetaTrader. Donc on envoit des requêtes, par exemple obtenir les informations des prix d'un symbol. Sinon on peut aussi demander d'ouvrir ou de modifier un trade.

Ensuite, on a un serveur WebSocket avec plusieurs behavior (end point) qui permette de rediriger les informations obtenu du TCP socket server vers le bon endpoint du WebSocket server.


# Pour lancer

Exécuter le exe suivant : solutionfx-app/SolutionFX/bin/Debug/DeveloperFX.exe
