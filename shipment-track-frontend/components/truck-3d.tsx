"use client"

import { Canvas } from "@react-three/fiber"
import { OrbitControls, PerspectiveCamera, Environment } from "@react-three/drei"
import { Suspense } from "react"

function Truck() {
  return (
    <group position={[0, -0.5, 0]}>
      {/* Truck Cabin */}
      <mesh position={[0, 0.8, 0.5]} castShadow>
        <boxGeometry args={[1.2, 1, 1.5]} />
        <meshStandardMaterial color="#dc2626" metalness={0.6} roughness={0.4} />
      </mesh>

      {/* Windshield */}
      <mesh position={[0, 1.1, 1.1]} castShadow>
        <boxGeometry args={[1, 0.6, 0.3]} />
        <meshStandardMaterial color="#1e3a8a" metalness={0.9} roughness={0.1} transparent opacity={0.6} />
      </mesh>

      {/* Truck Cargo Container */}
      <mesh position={[0, 0.9, -1.5]} castShadow>
        <boxGeometry args={[1.4, 1.4, 3]} />
        <meshStandardMaterial color="#1e3a8a" metalness={0.5} roughness={0.5} />
      </mesh>

      {/* Vertex Logo on Container */}
      <mesh position={[0, 1.2, -0.05]} rotation={[0, 0, 0]}>
        <planeGeometry args={[1, 0.4]} />
        <meshStandardMaterial color="#ffffff" />
      </mesh>

      {/* Front Wheels */}
      <mesh position={[-0.7, 0.25, 0.8]} rotation={[0, 0, Math.PI / 2]} castShadow>
        <cylinderGeometry args={[0.25, 0.25, 0.3, 16]} />
        <meshStandardMaterial color="#1a1a1a" />
      </mesh>
      <mesh position={[0.7, 0.25, 0.8]} rotation={[0, 0, Math.PI / 2]} castShadow>
        <cylinderGeometry args={[0.25, 0.25, 0.3, 16]} />
        <meshStandardMaterial color="#1a1a1a" />
      </mesh>

      {/* Rear Wheels */}
      <mesh position={[-0.7, 0.25, -2.2]} rotation={[0, 0, Math.PI / 2]} castShadow>
        <cylinderGeometry args={[0.25, 0.25, 0.3, 16]} />
        <meshStandardMaterial color="#1a1a1a" />
      </mesh>
      <mesh position={[0.7, 0.25, -2.2]} rotation={[0, 0, Math.PI / 2]} castShadow>
        <cylinderGeometry args={[0.25, 0.25, 0.3, 16]} />
        <meshStandardMaterial color="#1a1a1a" />
      </mesh>

      {/* Headlights */}
      <mesh position={[-0.4, 0.6, 1.3]}>
        <sphereGeometry args={[0.1, 16, 16]} />
        <meshStandardMaterial color="#ffff00" emissive="#ffff00" emissiveIntensity={2} />
      </mesh>
      <mesh position={[0.4, 0.6, 1.3]}>
        <sphereGeometry args={[0.1, 16, 16]} />
        <meshStandardMaterial color="#ffff00" emissive="#ffff00" emissiveIntensity={2} />
      </mesh>
    </group>
  )
}

export function Truck3D() {
  return (
    <div className="w-full h-[400px] md:h-[500px] rounded-lg overflow-hidden">
      <Canvas shadows>
        <Suspense fallback={null}>
          <PerspectiveCamera makeDefault position={[4, 2, 4]} fov={50} />
          <OrbitControls
            enableZoom={false}
            enablePan={false}
            autoRotate
            autoRotateSpeed={2}
            minPolarAngle={Math.PI / 3}
            maxPolarAngle={Math.PI / 2}
          />

          {/* Lighting */}
          <ambientLight intensity={0.5} />
          <directionalLight position={[5, 5, 5]} intensity={1} castShadow />
          <pointLight position={[-5, 5, 5]} intensity={0.5} />

          {/* Environment */}
          <Environment preset="sunset" />

          {/* Ground */}
          <mesh rotation={[-Math.PI / 2, 0, 0]} position={[0, -0.5, 0]} receiveShadow>
            <planeGeometry args={[20, 20]} />
            <meshStandardMaterial color="#334155" metalness={0.1} roughness={0.8} />
          </mesh>

          <Truck />
        </Suspense>
      </Canvas>
    </div>
  )
}
