import { NextResponse } from "next/server";

// Auth is enforced client-side in the (app) layout since the JWT lives in
// localStorage. This just normalizes the bare root path.
export function middleware() {
  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!_next/static|_next/image|favicon.ico).*)"],
};
