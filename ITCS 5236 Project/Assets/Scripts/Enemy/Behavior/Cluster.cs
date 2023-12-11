using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    // private GameManager manager;
    [SerializeField] private Transform myTransform;
    private List<GameObject> drops;
    private List<List<GameObject>> finalClusters;
        private List<GameObject> finalCentroids;
    private GameObject destination;
    [SerializeField] float centroidChangeRange;
    private int clusterNumbers;
   // [SerializeField] int carryCapacity;
    [SerializeField] GameObject centroidPrefab;

    // Start is called before the first frame update
    void Start() {
        // initialize manager
        // manager = GameObject.FindObjectWithTag("GameManager").GetComponent<GameManager>();

        // get a list of all the drops
        drops = new List<GameObject>(GameObject.FindGameObjectsWithTag("Drop"));
        foreach (GameObject drop in drops) {
            Debug.Log("*******Drop**********");
        }

        if(drops.Count == 1) {
            print("clusters = 1");
            clusterNumbers = 1;
        } else if (drops.Count < 5) {
            print("clusters = 2");
            clusterNumbers = 2;
        } else {
            print("clusters = 3");
            clusterNumbers = 3;
        }

        // run clustering algorithm to find where the clusters are on the screen
        finalCentroids = ClusterBehavior(drops, clusterNumbers);

        destination = GetDestination(finalCentroids);

    }

    /// <summary>
    /// Clustering algorithm that finds the location of k number of groups on the screen.
    /// </summary>
    /// <param name="objects">Objects that you want to group (i.e. drop objects)</param>
    /// <param name="numberOfClusters">Number of groupings</param>
    /// <returns>A list of sets of drops that indicate a grouping.</returns>
    private List<GameObject> ClusterBehavior(List<GameObject> objects, int numberOfClusters) {

        // CREATE N CENTROIDS AND RANDOMLY SET THEIR VALUES

        // declare max of random positions on world screen
        int x_max = 9;
        int y_max = 4;

        // Check if there are no objects to go to
        if(objects.Count == 0) {
            return new List<GameObject>();
        }

        List<Vector3> oldCentroids = new List<Vector3>();
        List<GameObject> centroids = new List<GameObject>();
        for(int i = 0; i < numberOfClusters; i++) {
            Vector3 randomLocation = new Vector3(Random.value * x_max, Random.value * y_max, 0.0f);
            GameObject newCentroid = Instantiate(centroidPrefab, randomLocation, Quaternion.identity);
            centroids.Add(newCentroid);
            oldCentroids.Add(randomLocation);

            Debug.Log("New Centroid Created With Coords of X: " + newCentroid.transform.position.x + " Y: " + newCentroid.transform.position.y );
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
            Debug.Log("ITERATION***********");

            // clear clusters for itteration
            clusters = ClearClusters(clusters);

            // keep track of closest cluster/centroid
            float bestDistance;        
            int indexOfBestDistance;
            
            foreach (GameObject point in objects) {
                bestDistance = Mathf.Infinity;
                indexOfBestDistance = -1;

                for (int i = 0; i < numberOfClusters; i++){
                    // distance between point and centroid i
                    float curDistance = EuclideanDistance(point.transform.position, centroids[i].transform.position);

                    // if current distance is smaller than best distance than change best distance
                    if(curDistance < bestDistance) {
                        bestDistance = curDistance;
                        indexOfBestDistance = i;
                    }
                }

                // Add point into closest cluster
                if(indexOfBestDistance == -1) {
                    Debug.Log("No best cluster was found for a drop");
                } else {
                    clusters[indexOfBestDistance].Add(point);
                }
            }


            // UPDATE CENTROID POSITION BASE ON AVERAGE POSITION OF POINTS IN CLUSTER
            oldCentroids.Clear();
            for(int i = 0; i < numberOfClusters; i++) {
                oldCentroids.Add(Clone(centroids[i]));
                // print("old list " + i + " : " + oldCentroids[i]);
            }

            
            centroids = UpdateCentroids(centroids, clusters);
            
            /*for(int i = 0; i < numberOfClusters; i++) {
                print("oldCentroid " + i + " : " + oldCentroids[i]);
                print("newCentroid " + i + " : " + centroids[i].transform.position);
                print(i + " cluster: " + clusters[i].Count);
            }*/

            // determine if we should break
            clusterDone = CentroidsInRange(centroids, oldCentroids, centroidChangeRange);
        }

        /*for (int i = 0; i < clusters.Count; i++) {
             print("cyan is cluster 0");
             print("magenta is cluster 1");
             print("yellow is cluster 2");

            foreach (GameObject drop in clusters[i]) {
                if(i == 0) {
                    drop.GetComponent<Renderer>().material.color = Color.cyan;
                }
                if(i == 1) {
                    drop.GetComponent<Renderer>().material.color = Color.magenta;
                }
                if(i == 2) {
                    drop.GetComponent<Renderer>().material.color = Color.yellow;
                }
            }
        }*/

        return centroids;

    }

    
    /// <summary>
    /// Finds the distance between two Vector3 points
    /// </summary>
    /// <param name="point">Location 1 to compare</param>
    /// <param name="centroid">Location 2 to compare</param>
    /// <returns>Distance between the point and centroid</returns>
        private float EuclideanDistance(Vector3 point, Vector3 centroid) {
        float x_distance = Mathf.Pow((centroid.x - point.x), 2.0f);
        float y_distance = Mathf.Pow((centroid.y - point.y), 2.0f);

        return Mathf.Sqrt(x_distance + y_distance);
    }

    /// <summary>
    /// Set each centroid to the avaregae of the location of all objects within it's current grouping
    /// </summary>
    /// <param name="centroids">Location to be updated</param>
    /// <param name="clusters">Current grouping of objects</param>
    /// <returns>Updated centroids list</returns>
    private List<GameObject> UpdateCentroids(List<GameObject> centroids, List<List<GameObject>> clusters) {
        
         // find the average points for each centroid
        float x_total;
        float y_total;

        // calculate the average of each cluster
        for (int i = 0; i < clusters.Count; i++) {
            x_total = 0.0f;
            y_total = 0.0f;

            // Compute the x and y totals positons for each point in a cluster
             foreach (GameObject point in clusters[i]) {
                x_total += point.transform.position.x;
                y_total += point.transform.position.y;
            }

            // set centroid to average of x and y total of a cluster
            if(clusters[i].Count != 0) {
                //print("Before centroid of cluster " + i + " : " + centroids[i].transform.position);
                Vector3 newCentroidLocation = new Vector3(x_total / clusters[i].Count, y_total / clusters[i].Count, 0);
                centroids[i].transform.position = newCentroidLocation;
                //print("Updated centroid positon " + i + " : " + centroids[i].transform.position);
            } else {
                //print("centroid " + i + " was empty");
            }

        }

        /*foreach (GameObject centroid in centroids) {
            Debug.Log("Centroid Offical Update: " + centroid.transform.position); 
        }*/

        return centroids;
    } 

    /// <summary>
    /// Empty cluster list of all objects for regrouping.
    /// </summary>
    /// <param name="clusters">Current grouping set up</param>
    /// <returns>Empty cluster lists</returns>
    private List<List<GameObject>> ClearClusters(List<List<GameObject>> clusters) {
        foreach (List<GameObject> cluster in clusters) {
            cluster.Clear();
        }
        return clusters;
    }

    /// <summary>
    /// Check if centroid location has drastically changed. If it has not, then stop running the
    /// clustering algorithm.
    /// </summary>
    /// <param name="newCentroids">List of new centroids</param>
    /// <param name="oldCentroids">List of old centroids</param>
    /// <param name="range">Determine the minimal distance needed to stop algorithm</param>
    /// <returns>True if the algorithm can stop and false if the algorithm must run again.</returns>
    private bool CentroidsInRange(List<GameObject> newCentroids, List<Vector3> oldCentroids, float range) {
        float[] centroidDifference = new float[clusterNumbers];

        // find the distance for every variable and retrun if out of range
        for(int i = 0; i < clusterNumbers; i++) {

            // Debug.Log("new centroid x: " + newCentroids[i]);
            // Debug.Log("old centroid x: " + oldCentroids[i]);
            centroidDifference[i] = EuclideanDistance(newCentroids[i].transform.position, oldCentroids[i]);
            
            // Debug.Log("Euclidean Difference: " + centroidDifference[i]);
            
           if(Mathf.Abs(centroidDifference[i]) > range) {
                // Debug.Log("Must go through another itteration");
                return false;
            }

            //Debug.Log("In range " + i);
        }

        // if all centroids are in range then return true
        return true;

    }

    /// <summary>
    /// Access the destination variable.
    /// </summary>
    /// <param name="clusters">Groups of drops
    /// <returns>Destination of closest cluster</returns>
    public GameObject GetDestination(List<GameObject> centroids) {
        
        // Test code for enemy movement
        GameObject destination;
        float bestDistance = Mathf.Infinity;
        int index = 0;

        for(int i = 0; i < clusterNumbers; i++) {
            float distance = EuclideanDistance(myTransform.position, centroids[i].transform.position);
            print(i + " cluster distance = " + distance);
            if(distance < bestDistance) {
                bestDistance = distance;
                index = i;
            }
        }

        destination = centroids[index];
        //destination.GetComponent<Renderer>().material.color = Color.green;

        print("going to centroid " + index);
        return destination;
    }

    public GameObject GetDestination() {
        return destination;
    }
    public Vector3 Clone(GameObject item) {
        Vector3 cloneItem = item.transform.position;
        return cloneItem;
    }

    public List<GameObject> GetCentroids() {
        return finalCentroids;
    }


}
