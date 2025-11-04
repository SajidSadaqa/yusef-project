"use client"

import { useRef } from "react"
import { Canvas, useFrame } from "@react-three/fiber"
import { useScroll } from "framer-motion"
import type * as THREE from "three"

function TruckModel() {
  const truckRef = useRef<THREE.Group>(null)
  const wheelsRef = useRef<THREE.Group[]>([])
  const { scrollYProgress } = useScroll()

  useFrame(() => {
    if (truckRef.current) {
      const scrollValue = scrollYProgress.get()
      truckRef.current.position.x = -10 + scrollValue * 20
      truckRef.current.position.y = -1.5 + Math.sin(scrollValue * Math.PI * 3) * 0.4
      truckRef.current.rotation.y = Math.PI / 8 + Math.sin(scrollValue * Math.PI) * 0.1

      // Rotate wheels based on movement
      wheelsRef.current.forEach((wheel) => {
        if (wheel) {
          wheel.rotation.x += 0.1
        }
      })
    }
  })

  return (
    <group ref={truckRef} position={[-10, -1.5, 0]}>
      {/* Main Cab Body */}
      <mesh position={[0, 0.6, 0]} castShadow>
        <boxGeometry args={[1.8, 1.4, 1.2]} />
        <meshStandardMaterial color="#dc2626" metalness={0.7} roughness={0.3} />
      </mesh>

      {/* Cab Roof/Sleeper */}
      <mesh position={[-0.3, 1.4, 0]} castShadow>
        <boxGeometry args={[1.2, 0.5, 1.2]} />
        <meshStandardMaterial color="#b91c1c" metalness={0.7} roughness={0.3} />
      </mesh>

      {/* Front Bumper */}
      <mesh position={[1, 0.2, 0]}>
        <boxGeometry args={[0.2, 0.4, 1.3]} />
        <meshStandardMaterial color="#374151" metalness={0.8} roughness={0.2} />
      </mesh>

      {/* Windshield */}
      <mesh position={[0.8, 0.8, 0]}>
        <boxGeometry args={[0.4, 0.8, 1.1]} />
        <meshStandardMaterial color="#1e3a8a" transparent opacity={0.6} metalness={0.9} roughness={0.1} />
      </mesh>

      {/* Side Windows */}
      <mesh position={[0.2, 0.8, 0.61]}>
        <boxGeometry args={[1, 0.6, 0.02]} />
        <meshStandardMaterial color="#1e3a8a" transparent opacity={0.5} metalness={0.9} roughness={0.1} />
      </mesh>
      <mesh position={[0.2, 0.8, -0.61]}>
        <boxGeometry args={[1, 0.6, 0.02]} />
        <meshStandardMaterial color="#1e3a8a" transparent opacity={0.5} metalness={0.9} roughness={0.1} />
      </mesh>

      {/* Main Trailer Body */}
      <mesh position={[-3, 1, 0]} castShadow>
        <boxGeometry args={[5, 2.2, 1.2]} />
        <meshStandardMaterial color="#2563eb" metalness={0.6} roughness={0.4} />
      </mesh>

      {/* Trailer Top Stripe */}
      <mesh position={[-3, 2, 0.61]}>
        <boxGeometry args={[5, 0.4, 0.02]} />
        <meshStandardMaterial color="#dc2626" metalness={0.5} roughness={0.5} />
      </mesh>
      <mesh position={[-3, 2, -0.61]}>
        <boxGeometry args={[5, 0.4, 0.02]} />
        <meshStandardMaterial color="#dc2626" metalness={0.5} roughness={0.5} />
      </mesh>

      {/* Trailer Door Details */}
      <mesh position={[-5.4, 1, 0]}>
        <boxGeometry args={[0.1, 2, 1.15]} />
        <meshStandardMaterial color="#1e40af" metalness={0.7} roughness={0.3} />
      </mesh>

      {/* Connection between cab and trailer */}
      <mesh position={[-1.2, 0.5, 0]}>
        <boxGeometry args={[0.3, 0.3, 0.3]} />
        <meshStandardMaterial color="#374151" metalness={0.8} roughness={0.2} />
      </mesh>

      {/* Front Wheels */}
      <group
        ref={(el) => {
          if (el) wheelsRef.current[0] = el
        }}
        position={[0.6, -0.2, 0.7]}
        rotation={[Math.PI / 2, 0, 0]}
      >
        <mesh castShadow>
          <cylinderGeometry args={[0.35, 0.35, 0.25, 20]} />
          <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.2} />
        </mesh>
        <mesh position={[0, 0, 0]}>
          <cylinderGeometry args={[0.15, 0.15, 0.26, 20]} />
          <meshStandardMaterial color="#6b7280" metalness={0.9} roughness={0.1} />
        </mesh>
      </group>
      <group
        ref={(el) => {
          if (el) wheelsRef.current[1] = el
        }}
        position={[0.6, -0.2, -0.7]}
        rotation={[Math.PI / 2, 0, 0]}
      >
        <mesh castShadow>
          <cylinderGeometry args={[0.35, 0.35, 0.25, 20]} />
          <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.2} />
        </mesh>
        <mesh position={[0, 0, 0]}>
          <cylinderGeometry args={[0.15, 0.15, 0.26, 20]} />
          <meshStandardMaterial color="#6b7280" metalness={0.9} roughness={0.1} />
        </mesh>
      </group>

      {/* Rear Cab Wheels (Dual) */}
      <group
        ref={(el) => {
          if (el) wheelsRef.current[2] = el
        }}
        position={[-0.8, -0.2, 0.7]}
        rotation={[Math.PI / 2, 0, 0]}
      >
        <mesh castShadow>
          <cylinderGeometry args={[0.35, 0.35, 0.25, 20]} />
          <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.2} />
        </mesh>
      </group>
      <group
        ref={(el) => {
          if (el) wheelsRef.current[3] = el
        }}
        position={[-0.8, -0.2, -0.7]}
        rotation={[Math.PI / 2, 0, 0]}
      >
        <mesh castShadow>
          <cylinderGeometry args={[0.35, 0.35, 0.25, 20]} />
          <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.2} />
        </mesh>
      </group>

      {/* Trailer Wheels - Front Set (Dual) */}
      <group
        ref={(el) => {
          if (el) wheelsRef.current[4] = el
        }}
        position={[-4, -0.2, 0.7]}
        rotation={[Math.PI / 2, 0, 0]}
      >
        <mesh castShadow>
          <cylinderGeometry args={[0.35, 0.35, 0.3, 20]} />
          <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.2} />
        </mesh>
      </group>
      <group
        ref={(el) => {
          if (el) wheelsRef.current[5] = el
        }}
        position={[-4, -0.2, -0.7]}
        rotation={[Math.PI / 2, 0, 0]}
      >
        <mesh castShadow>
          <cylinderGeometry args={[0.35, 0.35, 0.3, 20]} />
          <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.2} />
        </mesh>
      </group>

      {/* Trailer Wheels - Rear Set (Dual) */}
      <group
        ref={(el) => {
          if (el) wheelsRef.current[6] = el
        }}
        position={[-4.8, -0.2, 0.7]}
        rotation={[Math.PI / 2, 0, 0]}
      >
        <mesh castShadow>
          <cylinderGeometry args={[0.35, 0.35, 0.3, 20]} />
          <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.2} />
        </mesh>
      </group>
      <group
        ref={(el) => {
          if (el) wheelsRef.current[7] = el
        }}
        position={[-4.8, -0.2, -0.7]}
        rotation={[Math.PI / 2, 0, 0]}
      >
        <mesh castShadow>
          <cylinderGeometry args={[0.35, 0.35, 0.3, 20]} />
          <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.2} />
        </mesh>
      </group>

      {/* Headlights with glow */}
      <mesh position={[1.05, 0.5, 0.5]}>
        <sphereGeometry args={[0.1, 16, 16]} />
        <meshStandardMaterial color="#fbbf24" emissive="#fbbf24" emissiveIntensity={1} />
      </mesh>
      <mesh position={[1.05, 0.5, -0.5]}>
        <sphereGeometry args={[0.1, 16, 16]} />
        <meshStandardMaterial color="#fbbf24" emissive="#fbbf24" emissiveIntensity={1} />
      </mesh>
      <pointLight position={[1.2, 0.5, 0.5]} intensity={0.5} color="#fbbf24" distance={3} />
      <pointLight position={[1.2, 0.5, -0.5]} intensity={0.5} color="#fbbf24" distance={3} />

      {/* Exhaust Pipes */}
      <mesh position={[-0.5, 1.8, 0.4]} rotation={[0, 0, 0]}>
        <cylinderGeometry args={[0.08, 0.08, 1, 12]} />
        <meshStandardMaterial color="#4b5563" metalness={0.9} roughness={0.1} />
      </mesh>
      <mesh position={[-0.5, 1.8, -0.4]} rotation={[0, 0, 0]}>
        <cylinderGeometry args={[0.08, 0.08, 1, 12]} />
        <meshStandardMaterial color="#4b5563" metalness={0.9} roughness={0.1} />
      </mesh>

      {/* Grille */}
      <mesh position={[1.05, 0.5, 0]}>
        <boxGeometry args={[0.05, 0.6, 1]} />
        <meshStandardMaterial color="#1f2937" metalness={0.8} roughness={0.3} />
      </mesh>
    </group>
  )
}

export function BackgroundTruck() {
  return (
    <div className="fixed inset-0 pointer-events-none z-0 opacity-15">
      <Canvas camera={{ position: [0, 2, 12], fov: 50 }} style={{ background: "transparent" }} shadows>
        <ambientLight intensity={0.4} />
        <directionalLight position={[10, 10, 5]} intensity={1.2} castShadow />
        <directionalLight position={[-10, -10, -5]} intensity={0.4} />
        <spotLight position={[0, 10, 0]} intensity={0.5} angle={0.3} penumbra={1} />
        <TruckModel />
      </Canvas>
    </div>
  )
}
