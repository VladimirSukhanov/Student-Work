package main

import (
    "encoding/json"
    "log"
    "net/http"
    "strings"
)

// Sozdaem model polzovatelya
type User struct {
    ID       int    json:"id"
    Username string json:"username"
    Email    string json:"email"
}

// users - baza dannyh v pamyati
var users = []User{
    {ID: 1, Username: "Alise", Email: "alice@example.com"},
    {ID: 2, Username: "Ferdinant", Email: "Ferdinanti@example.com"},
}

// main - tochka vhoda
func main() {
    // Obrabotchik dlya GET
    http.HandleFunc("/users", getUsers)
    // Obrabotchik dlya POST - sozdaet novogo polzovatelya
    http.HandleFunc("/users/create", createUser)
    // Zapuskaem server na portu 8080
    log.Println("Server zapushen na http://localhost:8080")
    log.Fatal(http.ListenAndServe(":8080", nil))
}

func getUsers(w http.ResponseWriter, r http.Request) {
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(users)
}

func createUser(w http.ResponseWriter, rhttp.Request) {
    if r.Method != http.MethodPost {
        http.Error(w, "Metod ne podderzhivaetsya", http.StatusMethodNotAllowed)
        return
    }

    // Peremennaya kuda budem dekodirovat json
    var newUser User
    err := json.NewDecoder(r.Body).Decode(&newUser)

    if err != nil {
        http.Error(w, "Neverniy json", http.StatusBadRequest)
        return
    }

    // Validaciya (proverim chto polya ne pustye)
    if strings.TrimSpace(newUser.Username) == "" || strings.TrimSpace(newUser.Email) == "" {
        http.Error(w, "Username i email obyazatelny", http.StatusBadRequest)
        return
    }

    // Generiruem novyy id
    newId := users[len(users)-1].ID + 1
    newUser.ID = newId

    // Dobavlyaem polzovatelya v slice
    users = append(users, newUser)

    // Otpravlyaem otvet
    w.Header().Set("Content-Type", "application/json")
    w.WriteHeader(http.StatusCreated)
    json.NewEncoder(w).Encode(newUser)
}