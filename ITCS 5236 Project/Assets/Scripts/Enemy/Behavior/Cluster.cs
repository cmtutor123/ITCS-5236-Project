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


        // CREATE N CENTROIDS AND RANDOMLY SET THEIR VALUES

        // declare max of random positions on world screen
        int x_max = 1000;
        int y_max = 1000;

        List<Vector3> centroids = new List<Vector3>();

        for(int i = 0; i < numberOfClusters; i++) {
            centroids.Add(new Vector3(Random.value * x_max, Random.value * y_max, 0.0f));
        }

        // CREATE A LIST THAT HOLDS EACH CLUSTER (EACH INDEX CORRELATES TO CENTROIDS)
        List<List<GameObject>> clusters = new List<List<GameObject>>();
        for(int i = 0; i < numberOfClusters; i++) {
            clusters.Add(new List<GameObject>());
        }
        
        // GET THE DISTANCE BETWEEN EVERY POINT [GAME OBJECT] TO EVERY CENTROID
        // AND STORE POINT TO THE CLOSEST CLUSTER. (CLUSTER N CORRELATES TO CENTROID N).

        // keep track of closest cluster/centroid
        float bestDistance;        
        int indexOfBestDistance;
        
        foreach (GameObject point in objects) {
            bestDistance = Mathf.Infinity;
            indexOfBestDistance = 0;

            for (int i = 0; i < numberOfClusters; i++){
                // distance between point and centroid i
                float curDistance = EuclideanDistance(point.transform.position, centroids[i]);

                // if current distance is smaller than best distance than change best distance
                if(curDistance < bestDistance) {
                    bestDistance = curDistance;
                    indexOfBestDistance = i;
                }
            }

            // Add point into closest cluster
            clusters[indexOfBestDistance].Add(point);
        }


        // UPDATE CENTROID POSITION BASE ON AVERAGE POSITION OF POINTS IN CLUSTER

        // centroids = UpdateCentroids(centroids, clusters);
    }

    // return the distance between an object and a vector
    private float EuclideanDistance(Vector3 point, Vector3 centroid) {
        float x_distance = Mathf.Pow((point.x - centroid.x), 2.0f);
        float y_distance = Mathf.Pow((point.y - centroid.y), 2.0f);

        return Mathf.Sqrt(x_distance + y_distance);
    }

    // Update each centroid based on the average of the coinciding cluster
   /* private List<Vector3> UpdateCentroids(List<Vector3> centroids, List<List<GameObject>> clusters) {
        
         // find the average points for each centroid
        float x_total;
        float y_total;

        // Update every centroid
        for (int i = 0; i < clusters.Count; i++) {
            x_total = 0.0f;
            y_total = 0.0f;

            // Compute the x and y totals positons for each point in a cluster
             foreach (GameObject point in clusters[i]) {
                x_total += point.transform.position.x;
                y_total += point.transform.position.y;
             }

            // set centroid to average of x and y total of a cluster
            centroids[i].x = x_total / clusters[i].Count;
            centroids[i].y = y_total / clusters[i].Count;
        }

        
        return centroids;
    } */
}