using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    private GameManager manager;
    private List<GameObject> drops;

    // Start is called before the first frame update
    void Start() {
        // initialize manager
        manager = new GameManager();

        // get a list of all the drops
        drops = manager.GetDrops();


    }

    // Update is called once per frame
    void Update() {
        


    }


    private void ClusterBehavior(List<GameObject> objects, int numberOfClusters) {


        // declare min and max of random positions on world screen
        int x_max = 1000;
        int y_max = 1000;

        // create random centroids and store to a list
        List<Vector3> centroids = new List<Vector3>();
        for(int i = 0; i < numberOfClusters; i++) {
            centroids.Add(new Vector3(Random.value * x_max,Random.value * y_max, 0.0f));
        }

        // Create lists for each cluster (each index correlates to centroid)
        List<List<GameObject>> clusters = new List<List<GameObject>>();
        for(int i = 0; i < numberOfClusters; i++) {
            clusters.Add(new List<GameObject>());
        }
        
        // for every point [game object] get distance to each centroid

        float bestDistance;         // keep track of the best distance to a centroid for a point
        int indexOfBestDistance;
        foreach (GameObject point in objects) {
            bestDistance = Mathf.Infinity;
            indexOfBestDistance = 0;
            for (int i = 0; i < numberOfClusters; i++){
                // get distance between point and centroid
                float curDistance = EuclideanDistance(point.transform.position, centroids[i]);

                // if current distance is better best distance than change best distance
                if(curDistance < bestDistance) {
                    bestDistance = curDistance;
                    indexOfBestDistance = i;
                }
            }
            // put point into the cluster closest to
            clusters[indexOfBestDistance].Add(point);
        }

        // find the average points for each centroid/
        float x_total;
        float y_total;
        for (int i = 0; i < numberOfClusters; i++) {
            x_total = 0;
            y_total = 1;
            // for each cluster find the total of all x and y positions of the points
             foreach (GameObject point in clusters[i]) {
                x_total += point.transform.position.x;
                y_total += point.transform.position.y;
             }

            //average of X and Y of centroid
            //centroids[i].x = x_total / clusters[i].Count;
           // centroids[i].y = y_total / clusters[i].Count;
        }


    }

    // return the distance between an object and a vector
    private float EuclideanDistance(Vector3 point, Vector3 centroid) {
        float x_distance = Mathf.Pow((point.x - centroid.x), 2.0f);
        float y_distance = Mathf.Pow((point.y - centroid.y), 2.0f);

        return Mathf.Sqrt(x_distance + y_distance);
    }
}