# Animal Farm Framework

## Creators
Attila Kiss
and
Gábor Pusztai

Department of Information Systems, ELTE Eötvös Loránd University, Budapest 1117, Hungary

## Articles about this framework

**Planned: 2022. Q2.**

**Using the Unity game engine to develop 3D simulated ecologicalsystem based on a predator-prey model extended by geneevolution**

In the paper, we present a novel implementation of an ecosystem simulation. In our previous work, we implemented a 3D environment based on a predator-prey model, but we found that in most cases, regardless of the choice of starting parameters, the simulation quickly led to extinctions. We wanted to achieve system stabilization, long-term operation, and better simulation of reality by incorporating genetic evolution. Therefore we apply the predator-prey model with evolutional approach. Using Unity game engine we create and manage a 3D closed ecosystem environment defined by an artificial or real uploaded map. We present some demonstrational runs, while gathering data, observe interesting events (such as extinction, sustainability, behavior of swarms) and analyze possible effects on the initial parameters of the system. We found that incorporating genetic evolution into the simulation slightly stabilizes the system, reducing the likelihood of extinction of different types of objects. The simulation of ecosystems and the analysis of the data generated during the simulations can also be a starting point for further research, especially in relation to sustainability. Our system is publicly available, so anyone can customize and upload their own parameters, maps, objects, biological species and their inheritance and behavioral habits, so they can test their own hypotheses from the data generated during its operation. Another utilization of the system is education, students can learn in a playful and spectacular way how the ecosystem behaves, how natural selection helps the adaptability and survival of species, what effects overpopulation and competition can have.

<table border="0">
  <tr>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/Dom_Rec_Inheritance.png" alt="Dom_Rec_Inheritance" height="250" width="400"/></th>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/Food-chain.jpg" alt="Food-chain" height="250" width="400"/></th>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/AgeStages/AgeStages_whiteBG.jpg" alt="Food-chain" height="250" width="400"/></th>
  </tr>
  <tr>
    <td>Dominant-recessive inheritance</td>
    <td>Food chain</td>
    <td>Age stages</td>
  </tr>
  <tr>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/GeneTypes/Speed5.png" alt="Speed" height="160" width="400"/></td>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/GeneTypes/Reproduction.png" alt="Reproduction" height="160" width="400"/></td>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/GeneTypes/Metabolism.png" alt="Metabolism" height="160" width="400"/></td>
  </tr>
  <tr>
    <td>Speed gene</td>
    <td>Reproduction gene</td>
    <td>Metabolism gene</td>
  </tr>
  <tr>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/Phases/Exploring.png" alt="Exploring" height="160" width="400"/></td>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/Phases/MatingChildren.png" alt="MatingChildren" height="160" width="400"/></td>
    <td><img src="https://github.com/Wornox/AnimalFarmFramework/blob/main/PublishedArticles/Evo/Images/Phases/Metabolism.png" alt="Metabolism" height="160" width="400"/></td>
  </tr>
  <tr>
    <td>Exploring mechaninc</td>
    <td>Reproduction mechanic</td>
    <td>Metabolism mechanic</td>
  </tr>
</table>


---


**Published: 2021. June.**

**Animal Farm - a Complex Artificial Life 3D Framework**

http://www.acta.sapientia.ro/acta-info/C13-1/info13-1-4.pdf

The development of a computer-generated ecosystem simulations is becoming more common due to the greater computational capabilities of machines. Because natural ecosystems are very complex, simplifications are required for implementation. This simulation environment offer a global view of the system and generate a lot of data to process and analyse, which are difficult or impossible to do in nature. 3D simulations, besides of the scientific advantages in experiments, can be used for presentation, educational and entertainment purposes too. In our simulated framework (Animal Farm) we have implemented a few basic animal behaviors and mechanics to observe in 3D. All animals are controlled by an individual logic model which determines the animals next action based on their needs and surrounding environment.

## Releases
To try out the live version of the program without any installation, use the WebGl version : https://wornox.github.io/AnimalFarmWebGL/

To download runnable builds (Windows and Linux) : https://github.com/Wornox/AnimalFarmFramework/releases
