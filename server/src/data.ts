import axios from "axios";
import {RootObject} from "./models";
let dataLocal;
export const data = (): RootObject => dataLocal;
export const setup = async (): Promise<void> => dataLocal = (await axios.get("https://jobs.samuelbagattin.com/index.json")).data
