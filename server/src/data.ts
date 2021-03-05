import {RootObject} from "./models";
let dataLocal = {};
export const data = (): RootObject => <RootObject>dataLocal;
