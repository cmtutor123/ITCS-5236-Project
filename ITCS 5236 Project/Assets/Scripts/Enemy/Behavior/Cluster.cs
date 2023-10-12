using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    private GameManager manager;
    private List<GameObject> drops;
    private Vector3 destination;
    [SerializeField] float centroidChangeRange;
    [SerializeField] int clusterNumbers;
    [SerializeField] int carryCapacity;

    // Start is called before the first frame update
    void Start() {
        // initialize manager
        //manager = GameObject.FindObjectWithTag("GameManager").GetComponent<GameManager>();

        // get a list of all the drops
        //drops = manager.GetDrops();

        // run clustering algorithm and find destination
        //destination = ClusterBehavior(drops, clusterNumbers);

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
        List<Vector3> oldCentroids = new List<Vector3>();
        for(int i = 0; i < numberOfClusters; i++) {
            centroids.Add(new Vector3(Random.value * x_max, Random.value * y_max, 0.0f));
        }

        // CREATE A LIST THAT HOLDS EACH CLUSTER (EACH INDEX CORRELATES TO CENTROIDS)
        List<List<GameObject>> clusters = new List<List<GameObject>>();
        for(int i = 0; i < numberOfClusters; i++) {
            clusters.Add(new List<GameObject>());
        }
        
        // ITTERATE UNTIL THE CENTROIDS ARE IN THE CENTER OF THE CLUSTERS
        bool clusterDone = false;

        while(!clusterDone) {    
            // GET THE DISTANCE BETWEEN EVERY POINT [GAME OBJECT] TO EVERY CENTROID
            // AND STORE POINT TO THE CLOSEST CLUSTER. (CLUSTER N CORRELATES TO CENTROID N).

            // clear clusters for itteration
            clusters = ClearClusters(clusters);

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
            oldCentroids = centroids;
            centroids = UpdateCentroids(centroids, clusters);

            // determine if we should break
            clusterDone = CentroidsInRange(centroids, oldCentroids, centroidChangeRange);

            // find the final location to send AI
            //Vector3 finalLocation = getDestination(centroids, clusters);

            //return finalLocation;
        }




    }

    // return the distance between an object and a vector
    private float EuclideanDistance(Vector3 point, Vector3 centroid) {
        float x_distance = Mathf.Pow((point.x - centroid.x), 2.0f);
        float y_distance = Mathf.Pow((point.y - centroid.y), 2.0f);

        return Mathf.Sqrt(x_distance + y_distance);
    }

    // Update each centroid based on the average of the coinciding cluster
    private List<Vector3> UpdateCentroids(List<Vector3> centroids, List<List<GameObject>> clusters) {
        
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
            Vector3 tempCentroid = centroids[i];

            tempCentroid.x = x_total / clusters[i].Count;
            tempCentroid.y = y_total / clusters[i].Count;

            centroids[i] = tempCentroid;
        }

        
        return centroids;
    } 

    // clear the clusters to be reorganized
    private List<List<GameObject>> ClearClusters(List<List<GameObject>> clusters) {
        foreach (List<GameObject> cluster in clusters) {
            cluster.Clear();
        }
        return clusters;
    }

    // check if the centroid location is drastically changing. If not, then stop running the
    // clustering algorithm
    private bool CentroidsInRange(List<Vector3> newCentroids, List<Vector3> oldCentroids, float range) {
        float[] centroidDifference = new float[newCentroids.Count];

        // find the distance for every variable and retrun if out of range
        for(int i = 0; i < newCentroids.Count; i++) {
            centroidDifference[i] = EuclideanDistance(newCentroids[i], oldCentroids[i]);
            if(centroidDifference[i] > range) {
                return false;
            }
        }

        // if all centroids are in range then return true
        return true;

    }


    // determine best location to go based on distance and number of drops
   /* private Vector3 getDestination(List<Vector3> centroids, List<List<GameObject>> clusters) {
        // go to location with most amount of drops and closest

        float bestLocation = Mathf.Infinity;
        float currentLocation = Mathf.Infiniy;

        bestLocationIndex = 0;

        for (int i = 0; i < centroids.Count; i++) {

            // determine the weight of a current cluster
            currentLocation =  Mathf.Pow((EuclideanDistance(self, centroid) / clusters[i].Count), 2.0f);
            
            // if better location found then set as new destination
            if(currentLocation < bestLocation) {
                bestLocation = currentLocation;
                bestLocationIndex = i;
            } 

        }

        vector3 bestCentroidLocation = centorids[bestLocationIndex];

        retrun bestCentroidLocation;

    } */

}
